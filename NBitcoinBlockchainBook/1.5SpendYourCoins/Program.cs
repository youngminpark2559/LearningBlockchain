// ReSharper disable All
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace SpendYourCoins
{
    class Program
    {
        static void Main()
        {
            //Review how to set network, generate private key, bitcoin secret which is a private key represent in Base58Check.
            //var network = Network.TestNet;
            //Key privateKey = new Key();
            //var bitcoinPrivateKey = privateKey.GetWif(network);


            var bitcoinPrivateKey = new BitcoinSecret("cSZjE4aJNPpBtU6xvJ6J4iBzDgTmzTjbq8w2kqnYvAprBCyTsG4x");
            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();

            Console.WriteLine(bitcoinPrivateKey); // cSZjE4aJNPpBtU6xvJ6J4iBzDgTmzTjbq8w2kqnYvAprBCyTsG4x
            Console.WriteLine(network); //TestNet
            Console.WriteLine(address); // mzK6Jy5mer3ABBxfHdcxXEChsn3mkv8qJv
            Console.WriteLine();

            //Get the transaciton informations by using QBitNinja.
            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("e44587cf08b4f03b0e8b4ae7562217796ec47b8c91666681d71329b764add2e3");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            Console.WriteLine(transactionResponse.TransactionId); // e44587cf08b4f03b0e8b4ae7562217796ec47b8c91666681d71329b764add2e3
            Console.WriteLine(transactionResponse.Block.Confirmations);
            Console.WriteLine();


            //Now, we have every bit of informations we need to create our transacitons.
            //The main questions are "from where, to where, and how much?"

            //About "from where".
            //In this case example, we want to spend the second outpoint.
            //Recall that outpoint is composed of TransactionId and index of TxOut of the previous transaction.
            var receivedCoins = transactionResponse.ReceivedCoins;
            OutPoint outPointToSpend = null;
            foreach (var coin in receivedCoins)
            {
                if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                {
                    outPointToSpend = coin.Outpoint;
                }
            }
            if (outPointToSpend == null)
                throw new Exception("TxOut doesn't contain our ScriptPubKey");
            Console.WriteLine("We want to spend {0}. outpoint:", outPointToSpend.N + 1);

            //For the payment, you will need to reference this outpoint in the TxIn(inputs) of the transaciton. You create a transaciton as follows.
            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });


            //About "to where".
            //Constructing the TxIn and adding Txin to the transaciton is the answer to the "from where" question. 
            //Constructing the TxOut and adding TxOut to the transaciton is the answer to the "to where and how much".

            // var hallOfTheMakersAddress = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            //This is for testnet.
            var hallOfTheMakersAddress = new BitcoinPubKeyAddress("mzp4No5cmCXjZUpf112B1XWsvWBfws5bbB");

            // How much you want to TxOut.
            var hallOfTheMakersAmount = new Money((decimal)0.5, MoneyUnit.BTC);
            /* At the time of writing the mining fee is 0.05usd
             * Depending on the market price and
             * On the currently advised mining fee,
             * You may consider to increase or decrease it
            */
            var minerFee = new Money((decimal)0.0001, MoneyUnit.BTC);
            // How much you want to spend FROM
            //Get the entire coins that are retrieved from specific TxOut(in this case, second one) of previous transaction.
            var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
            Money changeBackAmount = txInAmount - hallOfTheMakersAmount - minerFee;



            //Let's add our calculated values to our TxOuts:
            TxOut hallOfTheMakersTxOut = new TxOut()
            {
                Value = hallOfTheMakersAmount,
                ScriptPubKey = hallOfTheMakersAddress.ScriptPubKey
            };

            TxOut changeBackTxOut = new TxOut()
            {
                Value = changeBackAmount,
                ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
            };

            //And add them to our transaction:
            transaction.Outputs.Add(hallOfTheMakersTxOut);
            transaction.Outputs.Add(changeBackTxOut);


            //Message on The Blockchain.
            //Message must be less than 40 bytes, or it will crash the application.
            //This message will appear as a feedback, along with your transaciton, after your transaciton is confirmed, in the Hall of The Makers site.
            var message = "nopara73 loves NBitcoin!";
            var bytes = Encoding.UTF8.GetBytes(message);
            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            });


            //To sum up, take a look at my whole transaciton before we sign it.
            Console.WriteLine(transaction);

            //prev_out n is 1. It's because we're indexing from 0, this means that I want to spend the second output of the previous transaciton.
            //In the blockexplorer, we can see the corresponding address which is mzK6Jy5mer3ABBxfHdcxXEChsn3mkv8qJv, and I can get the scriptSig from the address.
            //var address = new BitcoinPubKeyAddress("mzK6Jy5mer3ABBxfHdcxXEChsn3mkv8qJv");
            //transaction.Inputs[0].ScriptSig = address.ScriptPubKey;


            //Sign your transaction.
            //Now that we have created the transaciton, we must sign it.
            //In other words, you will have to prove that you own the TxOut that you referenced in the TxIn.
            //Signing can be complicated, but we'll make it simple.
            //First let's revisit the scriptSig of in, and how we can get it from code.
            //Remember, we copy/pasted the address above from a blockexplorer, now let's get it from our QBitNinja transacitonResponse.
            transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
            //Then you need to provide your private key in order to sign the transaciton.
            transaction.Sign(bitcoinPrivateKey, false);


            //Propagate your transactions.
            //After signed your transaction, you propagate the signed transaction to the network so that the miners can see it.

            //Propagate signed transaciton by QBitNinja
            BroadcastResponse broadcastResponse
            = client.Broadcast(transaction).Result;

            if (!broadcastResponse.Success)
            {
                Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            }
            else
            {
                Console.WriteLine("Success! You can check out the hash of the transaciton in any block explorer:");
                Console.WriteLine(transaction.GetHash());
            }


            ////Propagate signed transaciton by your own Bitcoin Core.
            ////Connect to the node.
            //using (var node = Node.ConnectToLocal(network)) 
            //{
            //    node.VersionHandshake(); //Say hello
            //                             //Advertize your transaction (send just the hash)
            //    node.SendMessage(new InvPayload(InventoryType.MSG_TX, transaction.GetHash()));
            //    //Send it
            //    node.SendMessage(new TxPayload(transaction));
            //    Thread.Sleep(500); //Wait a bit
            //}
        }
    }
}