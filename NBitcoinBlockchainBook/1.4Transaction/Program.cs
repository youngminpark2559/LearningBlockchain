// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using QBitNinja.Client;

namespace Transaction
{
    class Program
    {
        static void Main()
        {

            //========================================================================================
            //Section. Transaction

            //(Mastering Bitcoin) Transactions are the most important part of the bitcoin system. Everything else in bitcoin is designed to ensure that transactions can be created, propagated on the network, validated, and finally added to the global ledger of transactions(the blockchain).Transactions are data structures that encode the transfer of value between participants in the bitcoin system.Each transaction is a public entry in bitcoin’s blockchain, the global double-entry bookkeeping ledger.
            //A transaction may have no recipient, or it may have several.The same can be said for senders! On the Blockchain, the sender and recipient are always abstracted with a ScriptPubKey, as we demonstrated in previous chapters.
            //If you use Bitcoin Core, your Transactions tab will show the transaction, like this:
            //Picture depiction:




            //For now we are interested in the Transaction ID. In this case, it is: 
            //f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94


            //Note: The Transaction ID is defined by SHA256(SHA256(txbytes))


            //Note: Do NOT use the Transaction ID to handle unconfirmed transactions. The Transaction ID can be manipulated before it is confirmed.This is known as “Transaction Malleability.”


            //You can review or exmine a specify transaction on a block explorer like Blockchain.info:
            //https://blockchain.info/tx/f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94


            //But as a developer you will probably want a service that is easier to query and parse.
            //As a C# developer and an NBitcoin user Nicolas Dorier's QBit Ninja will definitely be your best choice. It is an open source web service API to query the blockchain and for tracking wallets.
            //QBit Ninja depends on NBitcoin.Indexer which relies on Microsoft Azure Storage.C# developers are expected to use the NuGet client package instead of developing a wrapper around this API.

            //If you go to:
            //http://api.qbit.ninja/transactions/f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94 
            //you will see the raw bytes of your transaction.
            //Picture depiction:



            //You can parse the transaction from a Hexadecimal with the following code:
            //Transaction tx = new Transaction("0100000...");



            //Quickly close the tab, before it scares you away. QBit Ninja queries the API and parses the information so go ahead and install QBitNinja.Client NuGet package.
            //Picture depiction:




            //Query the transaction by ID:
            //Create a client.
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);
            //Parse transaction ID to NBitcoin.uint256 so the client can eat it.
            var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
            Console.WriteLine($"transactionId: {transactionId}");
            //Query the transaction.
            QBitNinja.Client.Models.GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;
            Console.WriteLine($"transactionResponse: {transactionResponse}");
            //The type of transactionResponse is GetTransactionResponse. It lives under QBitNinja.Client.Models namespace. 

            //You can get NBitcoin.Transaction type from it:
            NBitcoin.Transaction transaction = transactionResponse.Transaction;
            Console.WriteLine($"transaction: {transaction}");

            //Let's see an example getting back the transaction ID with both classes(transactionResponse, transaction):
            Console.WriteLine(transactionResponse.TransactionId);
            //Output:
            //f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
            Console.WriteLine(transaction.GetHash());
            //Output:
            //f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94



            //            GetTransactionResponse has additional information about the transaction like the value and scriptPubKey of the inputs being spent in the transaction.
            //The relevant parts for now are the inputs and outputs.
            //You can see there is only one output in our transaction. 13.19683492 bitcoins are sent to that ScriptPubKey.



            //The relevant parts for now are the inputs and outputs.
            //You can see there is only one output in our transaction. 
            //13.19683492 bitcoins are sent to that ScriptPubKey.







            //Examine the RECEIVED COINS by using QBitNinja's GetTransactionResponse class
            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            Console.WriteLine("=====Examine the RECEIVED COINS by using QBitNinja's GetTransactionResponse class=====");
            foreach (var coin in receivedCoins)
            {
                Money amount = (Money)coin.Amount;

                Console.WriteLine($"amount.ToDecimal(MoneyUnit.BTC): {amount.ToDecimal(MoneyUnit.BTC)}");
                var paymentScript = coin.TxOut.ScriptPubKey;
                //Print each ScriptPubKey by executing foreach loop.
                Console.WriteLine(paymentScript);
                //Get a Bitcoin address.
                //Recall we can get a Bitcoin address from a ScriptPubKey by specifying a network type by processing backwards. 
                var bitcoinAddressWithQG = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine($"bitcoinAddressWithQG: {bitcoinAddressWithQG}");
                //Output:
                //1HfbwN6Lvma9eDsv7mdwp529tgiyfNr7jc
            }

            //Examine the RECEIVED COINS by using NBitcoin's Transaction class
            var outputs = transaction.Outputs;
            Console.WriteLine("=====Examine the received coins by using NBitcoin's Transaction class=====");
            foreach (TxOut output in outputs)
            {
                Coin coin = new Coin(transaction, output);
                Money amount = coin.Amount;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = coin.GetScriptCode();
                //It's the ScriptPubKey.
                Console.WriteLine(paymentScript);
                var bitcoinAddressWithNT = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine($"bitcoinAddressWithNT: {bitcoinAddressWithNT}");
                Console.WriteLine();
            }

            //We have written out some informations about the RECEIVED COINS using QBitNinja's GetTransactionResponse class and NBitcoins's Transaction class.




















            //Exercise: Write out the same informations about the SPENT COINS using QBitNinja's GetTransactionResponse class!
            //Examine the SPENT COINS by using QBitNinja's GetTransactionResponse class
            List<ICoin> spentCoins = transactionResponse.SpentCoins;
            Console.WriteLine("=====Examine the SPENT COINS by using QBitNinja's GetTransactionResponse class=====");
            foreach (var coin in spentCoins)
            {
                Money amount = (Money)coin.Amount;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = coin.TxOut.ScriptPubKey;
                Console.WriteLine(paymentScript);  // It's the ScriptPubKey
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
                Console.WriteLine();
            }

            //Examine the SPENT COINS by using NBitcoin's Transaction class
            Console.WriteLine("=====Examine the SPENT COINS by using NBitcoin's Transaction class=====");
            foreach (var coin in spentCoins)
            {
                TxOut previousOutput = coin.TxOut;
                Money amount = previousOutput.Value;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = previousOutput.ScriptPubKey;
                Console.WriteLine(paymentScript);  // It's the ScriptPubKey
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
                Console.WriteLine();
            }





            //===============================================================================
            //Examine the inputs.
            //Now let's examine the inputs. If you look at them, you will notice a previous output is referenced. Each input shows you which previous out has been spent in order to fund this transaction.

            Console.WriteLine("=====Examine the inputs=====");
            var inputs = transaction.Inputs;
            foreach (TxIn input in inputs)
            {
                //Get each previous output which is referenced to another transaction, from each input.
                OutPoint previousOutpoint = input.PrevOut;
                Console.WriteLine($"previousOutpoint: {previousOutpoint}");
                //Hash of the previous OutPoint.
                Console.WriteLine($"previousOutpoint.Hash: {previousOutpoint.Hash}");
                //Index number of the previous OutPoint that will be spent in the current transaction.
                Console.WriteLine($"previousOutpoint.N: {previousOutpoint.N}");
                Console.WriteLine();
            }

            //The terms TxOut, Output and out are synonymous.
            //Not to be confused with OutPoint, but more on this later.

            //In summary, the TxOut represents an amount of bitcoin and a ScriptPubKey representing a recipient.




            //As above illustration, let's create a TxOut with 21 bitcoins from the first ScriptPubKey in our current transaction:
            Money twentyOneBtc = new Money(21, MoneyUnit.BTC);
            //Get the first ScriptPubKey to specify a recipient.
            var scriptPubKey = transaction.Outputs.First().ScriptPubKey;
            //Create a new TxOut with passing twentyOneBtc, scriptPubKey into a constructor arguments.
            TxOut txOut = new TxOut(twentyOneBtc, scriptPubKey);
            Console.WriteLine($"Value in txOut: {txOut.Value}");
            //Output:
            //21.00000000
            Console.WriteLine($"ScriptPubKey in txOut: {txOut.ScriptPubKey}");
            //Output:
            //OP_DUP OP_HASH160 b6cefbb855cabf6ee45598f518a98011c22961aa OP_EQUALVERIFY OP_CHECKSIG




            //Every TxOut is uniquely addressed at the blockchain level by the ID of the transaction which includes TxOut(s) and TxOut(s) index number inside this transaction. We call such reference an Outpoint.
            //Picture depiction:
            //Transaction ID + Index number of TxOut => OutPoint


            //For example, the OutPoint of the TxOut with 13.19683492 BTC in our transaction is: 
            //(f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94, 0).
            //As you can see, OutPoint is composed of 2 values, the transaction ID including a TxOut and index number of the TxOut in the transaction.


            OutPoint firstOutPoint = receivedCoins.First().Outpoint;
            Console.WriteLine($"firstOutPoint.Hash {firstOutPoint.Hash}");
            //Output:
            //f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
            Console.WriteLine($"firstOutPoint.N: {firstOutPoint.N}");
            //Output:
            //0


            //Now let’s take a closer look at the inputs(aka TxIn) of the transaction:
            //Picture depiction:
            //OutPoint + ScriptSig => TxIn







            //The TxIn is composed of the "OutPoint" of the TxOut being spent in the current transaction including this TxIn, which originates in another previous transaction and the "ScriptSig". We can see the ScriptSig as the “Proof of Ownership”. In our transaction there are actually 9 inputs.

            Console.WriteLine(transaction.Inputs.Count);
            //Output:
            //9


            //With a transaction ID of the previous transaction including OutPoint, we can review the information associated with that transaction.
            OutPoint firstPreviousOutPoint = transaction.Inputs.First().PrevOut;
            Console.WriteLine($"firstPreviousOutPoint {firstPreviousOutPoint}");
            //Output:
            //4788c5ef8ffd0463422bcafdfab240f5bf0be690482ceccde79c51cfce209edd-0
            var firstPreviousTransactionResponse =
                client.GetTransaction(firstPreviousOutPoint.Hash).Result;
            Console.WriteLine($"firstPreviousTransactionResponse.TransactionId {firstPreviousTransactionResponse.TransactionId}");
            //Output:
            //4788c5ef8ffd0463422bcafdfab240f5bf0be690482ceccde79c51cfce209edd
            Console.WriteLine($"firstPreviousTransactionResponse.IsCoinbase: {firstPreviousTransactionResponse.IsCoinbase}");
            //Output:
            //False
            NBitcoin.Transaction firstPreviousTransaction = firstPreviousTransactionResponse.Transaction;
            Console.WriteLine($"firstPreviousTransaction {firstPreviousTransaction}");

            //CoinBase transaction is located in the first transaction, also known as Tx0, in a block. And it includes the newly mined coins and a transaction that the newly mied coins are sent to the miner.


            //We could continue to trace the transaction IDs back in this manner until we reach a coinbase transaction, the transaction including the newly mined coin by a miner.

            //Exercise: Follow the first input of this transaction and its ancestor transactions until you find a coinbase transaction!
            //Hint: After a few minutes and 30-40 transactions, I gave up tracing back.
            //Yes, you've guessed right, it is not the most efficient way to do this, but a good exercise.


            //while (firstPreviousTransactionResponse.IsCoinbase == false)
            //{
            //    Console.WriteLine(firstPreviousTransaction.GetHash());

            //    firstPreviousOutPoint = firstPreviousTransaction.Inputs.First().PrevOut;
            //    firstPreviousTransactionResponse = client.GetTransaction(firstPreviousOutPoint.Hash).Result;
            //    firstPreviousTransaction = firstPreviousTransactionResponse.Transaction;
            //}




            //In our example, the outputs were for a total of 13.19703492 BTC.
            Money spentAmount = Money.Zero;
            foreach (var spentCoin in spentCoins)
            {
                spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
            }
            Console.WriteLine($"spentAmount.ToDecimal(MoneyUnit.BTC): {spentAmount.ToDecimal(MoneyUnit.BTC)}");
            //Output:
            //13.19703492

            //In this transaction, 13.19683492 BTC were received.
            Money receivedAmount = Money.Zero;
            foreach (var receivedCoin in receivedCoins)
            {
                receivedAmount = (Money)receivedCoin.Amount.Add(receivedAmount);
            }
            Console.WriteLine($"receivedAmount.ToDecimal(MoneyUnit.BTC): {receivedAmount.ToDecimal(MoneyUnit.BTC)}");
            //Output:
            //13.19683492




            //Exercise: Get the total received amount, as I have been done with the spent amount.

            //var inputs = transaction.Inputs;
            //foreach (TxIn input in inputs)
            //{
            //    uint256 previousTransactionId = input.PrevOut.Hash;
            //    GetTransactionResponse previousTransactionResponse = client.GetTransaction(previousTransactionId).Result;

            //    NBitcoin.Transaction previousTransaction = previousTransactionResponse.Transaction;

            //    var previousTransactionOutputs = previousTransaction.Outputs;
            //    foreach (TxOut previousTransactionOutput in previousTransactionOutputs)
            //    {
            //        Money amount = previousTransactionOutput.Value;

            //        Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
            //        var paymentScript = previousTransactionOutput.ScriptPubKey;
            //        Console.WriteLine(paymentScript);  // It's the ScriptPubKey
            //        var address = paymentScript.GetDestinationAddress(Network.Main);
            //        Console.WriteLine(address);
            //        Console.WriteLine();
            //    }
            //}



            //That means 0.0002 BTC(or 13.19703492 - 13.19683492) is not accounted for!The difference between the inputs and outputs are called Transaction Fees or Miner’s Fees.This is the money that the miner collects for including a given transaction in a block.

            Console.WriteLine((spentAmount - receivedAmount).ToDecimal(MoneyUnit.BTC));
            Console.WriteLine(spentAmount.ToDecimal(MoneyUnit.BTC) - receivedAmount.ToDecimal(MoneyUnit.BTC));
            var fee = transaction.GetFee(spentCoins.ToArray());
            Console.WriteLine($"fee : {fee}");



            //You should note that a coinbase transaction is the only transaction whose value of output are superior to the value of input. This effectively corresponds to a coin creation. So by definition there is no fee in a coinbase transaction.The coinbase transaction is the first transaction of every block.
            //The consensus rules enforce that the sum of output's value in the coinbase transaction does not exceed the sum of transaction fees in the block plus the mining reward.

            Console.ReadLine();
        }
    }
}
