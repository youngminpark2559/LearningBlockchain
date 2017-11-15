//ReSharper disable All

using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin;
using NBitcoin.OpenAsset;

namespace _4._2LiquidDemocracy
{
    internal class Program
    {
        private static void Main()
        {
            //===========================================================================================
            //Section. Liquid Democracy

            //===========================================================================================
            //Section1. Overview
            //This part is a purely conceptual exercise of one application of colored coins.

            //Let’s imagine a company where some decisions are taken by a board of investors after a vote.
            //1.Some investors do not know enough about a topic, so they would like to delegate decisions about some subjects to someone else.
            //2.There is potentially a huge number of investors.
            //3.As the CEO, you want the ability to sell voting power for financing the company.
            //4.As the CEO, you want the ability to cast a vote when you decide.

            //How Colored Coins can help to organize such a vote transparently ?

            //But before beginning, let’s talk about some downside of voting on the Blockchain:
            //1.Nobody knows the real identity of a voter.
            //2.Miners could censor(even if it would be provable, and not in their interest.)
            //3.Even if nobody knows the real identity of the voter, behavioral analysis of a voter across several vote might reveal his identity.

            //Whether these points are relevant or not is up to the vote organizer to decide.

            //Let’s take an overview of how we would implement that.




            //===========================================================================================
            //Section1. Issuing voting power

            //Everything starts with the founder of the company(let’s call him Boss) wanting to sell “decision power” in his company to some investors. The decision power can take the shape of a colored coin that we will call for the sake of this exercise a “Power Coin”.

            //Let’s represent it in purple:



            //Let’s say that three persons are interested, Satoshi, Alice and Bob. (Yes, them again)
            //So Boss decides to sell each Power Coin at 0.1 BTC each.
            //Let’s start funding some money to the powerCoin address, satoshi, alice and bob.

            var powerCoin = new Key();
            var alice = new Key();
            var bob = new Key();
            var satoshi = new Key();
            var init = new Transaction()
            {
                Outputs =
                {
                    new TxOut(Money.Coins(1.0m), powerCoin),
                    new TxOut(Money.Coins(1.0m), alice),
                    new TxOut(Money.Coins(1.0m), bob),
                    new TxOut(Money.Coins(1.0m), satoshi),
                }
            };

            var repo = new NoSqlColoredTransactionRepository();
            repo.Transactions.Put(init);


            //Imagine that Alice buys 2 Power coins, here is how to create such transaction.
            var issuance = GetCoins(init, powerCoin)
                .Select(c => new IssuanceCoin(c))
                .ToArray();
            var builder = new TransactionBuilder();
            var toAlice =
                builder
                .AddCoins(issuance)
                .AddKeys(powerCoin)
                .IssueAsset(alice, new AssetMoney(powerCoin, 2))
                .SetChange(powerCoin)
                .Then()
                .AddCoins(GetCoins(init, alice))
                .AddKeys(alice)
                .Send(alice, Money.Coins(0.2m))
                .SetChange(alice)
                .BuildTransaction(true);
            repo.Transactions.Put(toAlice);
            //In summary, powerCoin issues 2 Power Coins to Alice and sends the change to himself(powerCoin). Likewise, Alice sends 0.2 BTC to powerCoin and sends the change to herself(Alice).


            //Where GetCoins method is
            //private IEnumerable<Coin> GetCoins(Transaction tx, Key owner)
            //{
            //    return tx.Outputs.AsCoins().Where(c => c.ScriptPubKey == owner.ScriptPubKey);
            //}


            //For some reason, Alice, might want to sell some of her voting power to Satoshi.






            var votingCoin = new Key();
            var init2 = new Transaction()
            {
                Outputs =
                    {
                        new TxOut(Money.Coins(1.0m), votingCoin),
                    }
            };
            repo.Transactions.Put(init2);

            issuance = GetCoins(init2, votingCoin).Select(c => new IssuanceCoin(c)).ToArray();
            builder = new TransactionBuilder();
            var toVoters =
                builder
                .AddCoins(issuance)
                .AddKeys(votingCoin)
                .IssueAsset(alice, new AssetMoney(votingCoin, 1))
                .IssueAsset(satoshi, new AssetMoney(votingCoin, 1))
                .SetChange(votingCoin)
                .BuildTransaction(true);
            repo.Transactions.Put(toVoters);

            var aliceVotingCoin = ColoredCoin.Find(toVoters, repo)
                        .Where(c => c.ScriptPubKey == alice.ScriptPubKey)
                        .ToArray();
            builder = new TransactionBuilder();
            var toBob =
                builder
                .AddCoins(aliceVotingCoin)
                .AddKeys(alice)
                .SendAsset(bob, new AssetMoney(votingCoin, 1))
                .BuildTransaction(true);
            repo.Transactions.Put(toBob);

            var bobVotingCoin = ColoredCoin.Find(toVoters, repo)
                        .Where(c => c.ScriptPubKey == bob.ScriptPubKey)
                        .ToArray();

            builder = new TransactionBuilder();
            var vote =
                builder
                .AddCoins(bobVotingCoin)
                .AddKeys(bob)
                .SendAsset(BitcoinAddress.Create("1HZwkjkeaoZfTSaJxDw6aKkxp45agDiEzN"),
                            new AssetMoney(votingCoin, 1))
                .BuildTransaction(true);

            issuance = GetCoins(init2, votingCoin).Select(c => new IssuanceCoin(c)).ToArray();
            issuance[0].DefinitionUrl = new Uri("http://boss.com/vote01.json");
            builder = new TransactionBuilder();
            toVoters =
                builder
                .AddCoins(issuance)
                .AddKeys(votingCoin)
                .IssueAsset(alice, new AssetMoney(votingCoin, 1))
                .IssueAsset(satoshi, new AssetMoney(votingCoin, 1))
                .SetChange(votingCoin)
                .BuildTransaction(true);
            repo.Transactions.Put(toVoters);

            Console.ReadLine();
        }

        private static IEnumerable<Coin> GetCoins(Transaction tx, Key owner)
        {
            return tx.Outputs.AsCoins().Where(c => c.ScriptPubKey == owner.ScriptPubKey);
        }

    }
}