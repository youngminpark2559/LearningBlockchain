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
            // Create a client
            QBitNinjaClient client = new QBitNinjaClient(Network.Main);
            // Parse transaction id to NBitcoin.uint256 so the client can eat it
            var transactionId = uint256.Parse("f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94");
            Console.WriteLine("aa" + transactionId);
            // Query the transaction
            QBitNinja.Client.Models.GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;


            NBitcoin.Transaction transaction = transactionResponse.Transaction;

            Console.WriteLine(transactionResponse.TransactionId); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94
            Console.WriteLine(transaction.GetHash()); // f13dc48fb035bbf0a6e989a26b3ecb57b84f85e0836e777d6edf60d87a4a2d94

            // RECEIVED COINS
            List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
            Console.WriteLine("=====Examine the received coins by using QBitNinja's GetTransactionResponse class=====");
            foreach (Coin coin in receivedCoins)
            {
                Money amount = coin.Amount;

                Console.WriteLine(amount.ToDecimal(MoneyUnit.BTC));
                var paymentScript = coin.ScriptPubKey;
                Console.WriteLine(paymentScript);  // It's the ScriptPubKey
                var address = paymentScript.GetDestinationAddress(Network.Main);
                Console.WriteLine(address);
                Console.WriteLine();
            }

            // RECEIVED COINS
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


            // SPENT COINS
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
