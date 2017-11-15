using System;
using System.Linq;
using NBitcoin;
using NBitcoin.OpenAsset;
// ReSharper disable All

namespace _4._1UnitTests
{
    internal class Program
    {
        private static void Main()
        {



            //========================================================================================
            //Section. Unit tests

            //You can see that previously I hard coded the properties of ColoredCoin.
            //The reason is that I wanted only to show you how to construct a Transaction out of ColoredCoin coins.
            //In real life, you would either depend on a third party API to fetch the colored coins of a transaction or a balance. Which might be not a good idea, because it adds a trust dependency to your program with the API provider.
            //NBitcoin allows you either to depend on a web service or to provide your own implementation for fetching the color of a Transaction. This allows you to have a flexible way to unit test your code, using another implementation or your own.
            //Let’s introduce two issuers: Silver and Gold. And three participants: Bob, Alice and Satoshi.
            //Let’s create a fake transaction that give some bitcoins to Silver, Gold and Satoshi.
            var gold = new Key();
            var silver = new Key();
            var goldId = gold.PubKey.ScriptPubKey.Hash.ToAssetId();
            var silverId = silver.PubKey.ScriptPubKey.Hash.ToAssetId();

            var bob = new Key();
            var alice = new Key();
            var satoshi = new Key();

            var init = new Transaction
            {
                Outputs =
                {
                    new TxOut("1.0", gold),
                    new TxOut("1.0", silver),
                    new TxOut("1.0", satoshi)
                }
            };

            //Init does not contain any Colored Coin issuance and Transfer. But imagine that you want to be sure of it, how would you proceed?
            //In NBitcoin, the summary of color transfers and issuances is described by a class called ColoredTransaction.

            //Picture depiction:
            //IBitcoinSerializable interface.
            //3 classes(ColoredTransaction, ColoredEntry, Asset) implement IBitcoinSerializable interface.


            //You can see that the ColoredTransaction class will tell you:
            //1.Which TxIn spends which Asset
            //2.Which TxOut emits which Asset
            //3.Which TxOut transfers which Asset

            //But the method that interests us right now is FetchColor, which will permit you to extract colored information out of the transaction you gave in input.
            //You see that it depends on a IColoredTransactionRepository.
            //In other words, IColoredTransactionRepository type object should be passed into FetchColor() method as one of the arguments of it.
            //Picture depiction.


            //IColoredTransactionRepository is only a store that will give you the ColoredTransaction from the txid. However, you can see that it depends on ITransactionRepository, which maps a Transaction id to its transaction.
            //An implementation of IColoredTransactionRepository is CoinprismColoredTransactionRepository which is a public API for colored coins operations.
            //However, you can easily do your own, here is how FetchColors method works.
            //The simplest case is: The IColoredTransactionRepository knows the color, in such case FetchColors method only returns that result.
            //Picture depiction:






//            The second case is that the IColoredTransactionRepository does not know anything about the color of the transaction.
//So FetchColors will need to compute the color itself according to the open asset specification.
//However, for computing the color, FetchColors need the color of the parent transactions.
//So it fetch each of them on the ITransactionRepository, and call FetchColors on each of them.
//Once FetchColors has resolved the color of the parent’s recursively, it computes the transaction color, and caches the result back in the IColoredTransactionRepository.


            var repo = new NoSqlColoredTransactionRepository();

            repo.Transactions.Put(init);

            ColoredTransaction color = ColoredTransaction.FetchColors(init, repo);
            Console.WriteLine(color);

            var issuanceCoins =
                init
                    .Outputs
                    .AsCoins()
                    .Take(2)
                    .Select((c, i) => new IssuanceCoin(c))
                    .OfType<ICoin>()
                    .ToArray();

            var builder = new TransactionBuilder();
            var sendGoldToSatoshi =
                builder
                .AddKeys(gold)
                .AddCoins(issuanceCoins[0])
                .IssueAsset(satoshi, new AssetMoney(goldId, 10))
                .SetChange(gold)
                .BuildTransaction(true);
            repo.Transactions.Put(sendGoldToSatoshi);
            color = ColoredTransaction.FetchColors(sendGoldToSatoshi, repo);
            Console.WriteLine(color);

            var goldCoin = ColoredCoin.Find(sendGoldToSatoshi, color).FirstOrDefault();

            builder = new TransactionBuilder();
            var sendToBobAndAlice =
                    builder
                    .AddKeys(satoshi)
                    .AddCoins(goldCoin)
                    .SendAsset(alice, new AssetMoney(goldId, 4))
                    .SetChange(satoshi)
                    .BuildTransaction(true);


            var satoshiBtc = init.Outputs.AsCoins().Last();
            builder = new TransactionBuilder();
            var sendToAlice =
                    builder
                    .AddKeys(satoshi)
                    .AddCoins(goldCoin, satoshiBtc)
                    .SendAsset(alice, new AssetMoney(goldId, 4))
                    .SetChange(satoshi)
                    .BuildTransaction(true);
            repo.Transactions.Put(sendToAlice);
            color = ColoredTransaction.FetchColors(sendToAlice, repo);

            Console.WriteLine(sendToAlice);
            Console.WriteLine(color);

            Console.ReadLine();
        }
    }
}