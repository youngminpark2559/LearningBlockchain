using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

//c Add codes. I use main network, and with that network I generate a bitcoinPrivateKey by being helped by GetWif(). I get the address of bitcoinPrivateKey.
//c Add codes. I create bitcoinPrivateKey by using BitcoinSecret object instantiated with hash. I get the network and address of this bitcoinPrivateKey.
//c Add codes. I create a QBitNinjaClient object with passing network that I'm using here. I get a specific transactionId by parsing a specific hash by using uint256.Parse(). I get the transaction by invoking GetTransaction of QBitNinjaClient type with passing transactionId.
//c Add codes. I get receivedCoins from transactionResponse. I create OutPoint object to hold data. If ScriptPubKey of receivedCoins is identical to ScriptPubKey of bitcoinPrivateKey, I store OutPoint of receivedCoins to outPointToSpend. And finally I add TxIn object which contains outPointToSpend in PrevOut into transaction object.
//c Additionaly comment. Constructing the TxIn and adding this TxIn to the transaction is the answer to the "from where" question. Constructing the TxOut and adding this TxOut to the transaction is the answer to the "remaining questsions, "to where, how much".
//c Add codes for examining "to where, how much" questsions.
//c Add codes for message which will appear with transaction.

namespace SpendYourCoins
{
    class Program
    {
        static void Main()
        {
            // Create a new private key.
            // var network = Network.TestNet;
            RandomUtils.Random = new UnsecureRandom();
            Key privateKey = new Key();
            // var bitcoinPrivateKey = privateKey.GetWif(network);


            //Console.WriteLine(bitcoinPrivateKey);
            //Console.WriteLine(address);



            var bitcoinPrivateKey = new BitcoinSecret("cSZjE4aJNPpBtU6xvJ6J4iBzDgTmzTjbq8w2kqnYvAprBCyTsG4x");
            var network = bitcoinPrivateKey.Network;
            var address = bitcoinPrivateKey.GetAddress();


            Console.WriteLine(bitcoinPrivateKey); // cSZjE4aJNPpBtU6xvJ6J4iBzDgTmzTjbq8w2kqnYvAprBCyTsG4x
            Console.WriteLine(network);
            Console.WriteLine(address); // mzK6Jy5mer3ABBxfHdcxXEChsn3mkv8qJv
            Console.WriteLine();

            //I get the transaction information.
            //After these tasks have been done, I have every bit of information which I need to create my transactions.
            //And the main question is going to be about "from where, to where, and how much".
            var client = new QBitNinjaClient(network);
            var transactionId = uint256.Parse("e44587cf08b4f03b0e8b4ae7562217796ec47b8c91666681d71329b764add2e3");
            var transactionResponse = client.GetTransaction(transactionId).Result;

            Console.WriteLine(transactionResponse.TransactionId); // e44587cf08b4f03b0e8b4ae7562217796ec47b8c91666681d71329b764add2e3
            Console.WriteLine(transactionResponse.Block.Confirmations);
            Console.WriteLine();






            //About "from where?".
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

            var transaction = new Transaction();
            transaction.Inputs.Add(new TxIn()
            {
                PrevOut = outPointToSpend
            });


            //About "to where" question.
            //Create BitcoinPubKeyAddress object with passing the donation address of this book. 
            //var hallOfTheMakersAddress = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            //If I'm working on the testnet, I can send the testnet coins to any testnet address.
            var hallOfTheMakersAddress = new BitcoinPubKeyAddress("mzp4No5cmCXjZUpf112B1XWsvWBfws5bbB");




            //About "how much" question.
            //Set I want to send 0.5
            var hallOfTheMakersAmount = new Money((decimal)0.5, MoneyUnit.BTC);
            //Set the mining fee.
            //At the time of writing the mining fee is 0.05usd.
            //Depending on the market price and on the currently advised mining fee,
            //you may consider to increase or decrease it.
            var minerFee = new Money((decimal)0.0001, MoneyUnit.BTC);

            // How much you want to spend FROM.
            //Suppose I have 1BTC.
            var txInAmount = (Money)receivedCoins[(int)outPointToSpend.N].Amount;
            //I'll get back as much as changeBackAmount(0.4999 = 1 - 0.5 - 0.0001)
            Money changeBackAmount = txInAmount - hallOfTheMakersAmount - minerFee;


            //Add our calculated values to our TxOuts.
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

            //Add them to our transaction
            transaction.Outputs.Add(hallOfTheMakersTxOut);
            transaction.Outputs.Add(changeBackTxOut);



            //Codes for message which will appear with transaction.
            var message = "nopara73 loves NBitcoin!";
            var bytes = Encoding.UTF8.GetBytes(message);
            transaction.Outputs.Add(new TxOut()
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(bytes)
            });



            //Console.WriteLine(transaction);

            ////var address = new BitcoinPubKeyAddress("mzK6Jy5mer3ABBxfHdcxXEChsn3mkv8qJv");
            ////transaction.Inputs[0].ScriptSig = address.ScriptPubKey;

            //// It is also OK:
            //transaction.Inputs[0].ScriptSig = bitcoinPrivateKey.ScriptPubKey;
            //transaction.Sign(bitcoinPrivateKey, false);

            //BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

            //if (!broadcastResponse.Success)
            //{
            //    Console.WriteLine(string.Format("ErrorCode: {0}", broadcastResponse.Error.ErrorCode));
            //    Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            //}
            //else
            //{
            //    Console.WriteLine("Success! You can check out the hash of the transaction in any block explorer:");
            //    Console.WriteLine(transaction.GetHash());
            //}





            //using (var node = Node.ConnectToLocal(network)) //Connect to the node
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
