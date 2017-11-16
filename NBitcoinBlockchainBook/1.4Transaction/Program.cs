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







            //Received coins.

            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            Console.WriteLine("=====Examine the received coins by using QBitNinja's GetTransactionResponse class=====");
            foreach (var coin in receivedCoins)
            {
                Money amount = (Money)coin.Amount;

                Console.WriteLine($"amount.ToDecimal(MoneyUnit.BTC): {amount.ToDecimal(MoneyUnit.BTC)}");
                var paymentScript = coin.TxOut.ScriptPubKey;
                //Print each ScriptPubKey by executing foreach.
                Console.WriteLine(paymentScript);
                //Get a Bitcoin address.
                //Recall we can get a Bitcoin address by processing backwards from a ScriptPubKey by specifying a network type. 
                var bitcoinAddress = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(bitcoinAddress);
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
                Console.WriteLine(paymentScript);  // It's the ScriptPubKey
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
                Console.WriteLine();
            }

            //We have written out some informations about the RECEIVED COINS using QBitNinja's GetTransactionResponse class.




















            //Exercise: Write out the same informations about the SPENT COINS using QBitNinja's GetTransactionResponse class!
            //Examine the SPENT COINS by using QBitNinja's GetTransactionResponse class
            List<ICoin> spentCoins = transactionResponse.SpentCoins;
            Console.WriteLine("=====Examine the spentCoins by using QBitNinja's GetTransactionResponse class=====");
            foreach (Coin coin in spentCoins)
            {
                Money amount = coin.Amount;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = coin.ScriptPubKey;
                Console.WriteLine(paymentScript);  // It's the ScriptPubKey
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
                Console.WriteLine();
            }

            // SPENT COINS
            Console.WriteLine("=====Examine the spentCoins by using NBitcoin's Transaction class=====");
            foreach (Coin coin in spentCoins)
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


            var fee = transaction.GetFee(spentCoins.ToArray());
            Console.WriteLine($"fee : {fee}");

            Console.WriteLine("=====Examine the inputs=====");
            var inputs = transaction.Inputs;
            foreach (TxIn input in inputs)
            {
                //Get each previous output from each input.
                OutPoint previousOutpoint = input.PrevOut;
                Console.WriteLine(previousOutpoint.Hash); // hash of prev tx
                Console.WriteLine(previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
                Console.WriteLine();
            }

            //The term TxOut, Output, out are synonymous.
            //TxOut is composed of Bitcoin amount(value) and ScriptPubKey for recipient.
            // Let's create a txout with 21 bitcoin from the first ScriptPubKey in our current transaction
            Money twentyOneBtc = new Money(21, MoneyUnit.BTC);
            var scriptPubKey = transaction.Outputs.First().ScriptPubKey;
            TxOut txOut = new TxOut(twentyOneBtc, scriptPubKey);


            //Every TxOut is uniquely addressed at the blockchain level by the ID of the transaction. And specific transaction by such ID includes TxOut and TxOut's index. And we call such reference an Outpoint.
            //For example, the Outpoint of the TxOut with 13.19683492 BTC in our transaction is //(f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94, 0).
            //(Transaction Id, Index of TxOut)

            OutPoint firstOutPoint0 = receivedCoins.First().Outpoint;
            Console.WriteLine($"firstOutPoint.Hash0 {firstOutPoint0.Hash}");
            //This is transactionId.
            // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
            Console.WriteLine(firstOutPoint0.N); // 0



            OutPoint firstOutPoint = spentCoins.First().Outpoint;
            Console.WriteLine($"firstOutPoint.Hash {firstOutPoint.Hash}");
            //This is previousOutpoint.Hash.
            // 4788c5ef8ffd0463422bcafdfab240f5bf0be690482ceccde79c51cfce209edd
            Console.WriteLine(firstOutPoint.N); // 0




            //TxIn(inputs) is composed of the OutPoint of the TxOut being spent and the ScriptSig(we can see the ScriptSig as the "Proof of Ownership").
            //In this transaction, there are 9 inputs.
            Console.WriteLine(transaction.Inputs.Count); // 9

            OutPoint firstPreviousOutPoint = transaction.Inputs.First().PrevOut;
            Console.WriteLine($"firstPreviousOutPoint {firstPreviousOutPoint}");
            //Output:
            //4788c5ef8ffd0463422bcafdfab240f5bf0be690482ceccde79c51cfce209edd-0
            var firstPreviousTransactionResponse = 
                client.GetTransaction(firstPreviousOutPoint.Hash).Result;
            Console.WriteLine($"firstPreviousTransactionResponse.TransactionId {firstPreviousTransactionResponse.TransactionId}");
            //Output:
            //4788c5ef8ffd0463422bcafdfab240f5bf0be690482ceccde79c51cfce209edd
            Console.WriteLine(firstPreviousTransactionResponse.IsCoinbase); // False
            NBitcoin.Transaction firstPreviousTransaction = firstPreviousTransactionResponse.Transaction;
            Console.WriteLine($"firstPreviousTransaction {firstPreviousTransaction}");


            //while (firstPreviousTransactionResponse.IsCoinbase == false)
            //{
            //    Console.WriteLine(firstPreviousTransaction.GetHash());

            //    firstPreviousOutPoint = firstPreviousTransaction.Inputs.First().PrevOut;
            //    firstPreviousTransactionResponse = client.GetTransaction(firstPreviousOutPoint.Hash).Result;
            //    firstPreviousTransaction = firstPreviousTransactionResponse.Transaction;
            //}

            Money spentAmount = Money.Zero;
            foreach (var spentCoin in spentCoins)
            {
                spentAmount = (Money)spentCoin.Amount.Add(spentAmount);
            }
            Console.WriteLine(spentAmount.ToDecimal(MoneyUnit.BTC)); // 13.19703492

            Money receivedAmount = Money.Zero;
            foreach (var receivedCoin in receivedCoins)
            {
                receivedAmount = (Money)receivedCoin.Amount.Add(receivedAmount);
            }
            Console.WriteLine(receivedAmount.ToDecimal(MoneyUnit.BTC)); // 13.19683492

            Console.WriteLine((spentAmount - receivedAmount).ToDecimal(MoneyUnit.BTC));

            Console.WriteLine(spentAmount.ToDecimal(MoneyUnit.BTC) - receivedAmount.ToDecimal(MoneyUnit.BTC));







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

            Console.ReadLine();
        }
    }
}
