// ReSharper disable All
using System;
using System.Threading;
using NBitcoin;
using NBitcoin.DataEncoders;
using NBitcoin.OpenAsset;
using NBitcoin.Protocol;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace _4._1OtherAssets
{
    class Program
    {
        static void Main()
        {
            //==========================================================================================
            //Other types of asset

            //In the previous chapters, we have seen several types of ownership. You have seen all the different kind of ownership and proof of ownership, and understand how Bitcoin can be coded to invent new kinds of ownership.





            //==========================================================================================
            //Colored Coins

            //So until now, you have seen how to exchange Bitcoins on the network. However you can use the Bitcoin network for transferring and exchanging any type of assets.
            //We call such assets “colored coins”.
            //As far as the Blockchain is concerned, there is no difference between a Coin and a Colored Coin.
            //A colored coin is represented by a standard TxOut.Most of the time, such TxOut have a residual Bitcoin value called “Dust”. (600 satoshi)
            //The real value of a colored coin reside in what the issuer of the coin will exchange against it.


            //Since a colored coin is nothing but a standard coin with special meaning, it follows that all what you saw about proof of ownership and the TransactionBuilder stays true.You can transfer a colored coin with exactly the same rules as before.
            //As far as the blockchain is concerned, a Colored Coin is a Coin like all others.
            //You can represent several type of asset with a colored coin: company shares, bonds, stocks, votes.
            //But no matter what type of asset you will represent, there will always have a trust relationship between the issuer of the asset and the owner.
            //If you own some company share, then the company might decide to not send you dividends.
            //If you own a bond, then the bank might not exchange it at maturity.
            //However, a violation of contract might be automatically detected with the help of Ricardian Contracts.
            //A Ricardian Contract is a contract signed by the issuer with the rights attached to the asset. Such contract can be not only human-readable (PDF) but also structured (JSON), so tools can automatically prove any violation.
            //The issuer can’t change the ricardian contract attached to an asset.
            //The Blockchain is only the transport medium of a financial instrument.
            //The innovation is that everyone can create and transfer its own asset without intermediary, whereas traditional asset transport mediums (clearing houses), are either heavily regulated or purposefully kept secret, and closed to the general public.
            //Open Asset is the name of the protocol created by Flavien Charlon that describes how to transfer and emit colored coins on the Blockchain.
            //Other protocols exist, but Open Asset is the most easy and flexible and the only one supported by NBitcoin.
            //In the rest of the book, I will not go in the details of the Open Asset protocol, the GitHub page of the specification is better suited to this need.




            //===========================================================================================
            //Section. Issuing an Asset

            //===========================================================================================
            //Section1. Objective

            //For the purpose of this exercise, I will emit BlockchainProgramming coins.
            //You get one of these BlockchainProgramming coins for every 0.004 bitcoin you send me.
            //One more if you add some kind words.
            //Furthermore, this is a great opportunity to make it to the Hall of The Makers.
            //Let’s see how I would code such feature.




            //==========================================================================================
            //Section2. Issuance Coin
            //In Open Asset, the Asset ID is derived from the issuer's ScriptPubKey.
            //If you want to issue a Colored Coin, you need to prove ownership of such ScriptPubKey.And the only way to do that on the Blockchain is by spending a coin belonging to such ScriptPubKey.
            //The coin that you will choose to spend for issuing colored coins is called “Issuance Coin” in NBitcoin.
            //I want to emit an Asset from this book's bitcoin address: 1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB.
            //Take a look at my balance by some blockchain explorers, I decided to use the following coin(200,000 satoshis) for issuing assets.

            //{
            //       "transactionId": "eb49a599c749c82d824caf9dd69c4e359261d49bbb0b9d6dc18c59bc9214e43b",
            //       "index": 0,
            //       "value": 2000000,
            //       "scriptPubKey": "76a914c81e8e7b7ffca043b088a992795b15887c96159288ac",
            //       "redeemScript": null
            //}


            //Here is how to create my issuance coin:
            var coin = new Coin(
                 fromTxHash: new uint256("eb49a599c749c82d824caf9dd69c4e359261d49bbb0b9d6dc18c59bc9214e43b"),
                 fromOutputIndex: 0,
                 amount: Money.Satoshis(2000000),
                 scriptPubKey: new Script(Encoders.Hex.DecodeData("76a914c81e8e7b7ffca043b088a992795b15887c96159288ac")));

            var issuance = new IssuanceCoin(coin);

            //Now I need to build transaction and sign the transaction with the help of the TransactionBuilder.
            var nico = BitcoinAddress.Create("15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe");
            //var bookKey = new BitcoinSecret("???????");
            var bookKey = new Key().GetBitcoinSecret(Network.Main); // Just a fake key in order to not get an exception

            TransactionBuilder builder = new TransactionBuilder();

            Transaction tx = builder
                .AddKeys(bookKey)
                .AddCoins(issuance)
                .IssueAsset(nico, new AssetMoney(issuance.AssetId, quantity: 10))
                .SendFees(Money.Coins(0.0001m))
                .SetChange(bookKey.GetAddress())
                .BuildTransaction(true);

            Console.WriteLine(tx);
            //Output:
            //{
            //  …
            //  "out": [
            //    {
            //      "value": "0.00000600",
            //      "scriptPubKey": "OP_DUP OP_HASH160 356facdac5f5bcae995d13e667bb5864fd1e7d59 OP_EQUALVERIFY OP_CHECKSIG"
            //    },
            //    {
            //      "value": "0.01989400",
            //      "scriptPubKey": "OP_DUP OP_HASH160 c81e8e7b7ffca043b088a992795b15887c961592 OP_EQUALVERIFY OP_CHECKSIG"
            //    },
            //    {
            //      "value": "0.00000000",
            //      "scriptPubKey": "OP_RETURN 4f410100010a00"
            //    }
            //  ]
            //}

            //You can see it includes an OP_RETURN output. In fact, this is the location where information about colored coins are stuffed.
            //Here is the format of the data in the OP_RETURN.
            //Picture depiction:
            //IBitcoinSerializable. ColorMarker class. Of the class, properties and methods.



            //In our case, Quantities have only 10, which is the number of Asset I issued to nico. Metadata is arbitrary data. We will see that we can put an url that points to an “Asset Definition”.
            //An Asset Definition is a document that describes what the Asset is.It is optional, so we are not using it in our case. (We’ll come back later on it in the Ricardian Contract part.)
            //For more information check out the Open Asset Specification.
            //After transaction verifications it is ready to be sent to the network.

            //Trasaction verification.
            Console.WriteLine(builder.Verify(tx));



            //=============================================================================================
            //Section3. With QBitNinja

            //We can do the same thing by a QBitNinja.
            //var client = new QBitNinjaClient(Network.Main);
            //BroadcastResponse broadcastResponse = client.Broadcast(tx).Result;

            //if (!broadcastResponse.Success)
            //{
            //    Console.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
            //    Console.WriteLine("Error message: " + broadcastResponse.Error.Reason);
            //}
            //else
            //{
            //    Console.WriteLine("Success!");
            //}


            //=============================================================================================
            //Section4. Or With local Bitcoin Core

            //We can do the same thing by a local Bitcoin Core.
            ////Connect to the node
            //using (var node = Node.ConnectToLocal(Network.Main)) 
            //{
            //    //Say hello
            //    node.VersionHandshake(); 
            //    //Advertize your transaction (send just the hash)
            //    node.SendMessage(new InvPayload(InventoryType.MSG_TX, tx.GetHash()));
            //    //Send it
            //    node.SendMessage(new TxPayload(tx));
            //    //Wait a bit
            //    Thread.Sleep(500); 
            //}

            //My Bitcoin Wallet has both, the book address and the “Nico”'s address.
            //Picture depiction:




            //As you can see, Bitcoin Core only shows the 0.0001 BTC of fees I paid, and ignores the 200,000 satoshies coin because of spam prevention feature.
            //This classical bitcoin wallet knows nothing about Colored Coins.
            //Worse: If a classical bitcoin wallet spends a colored coin, it will destroy the underlying asset and transfer only the bitcoin value of the TxOut. (200,000 satoshis)
            //For preventing a user from sending Colored Coin to a wallet that does not support it, Open Asset has its own address format, that only colored coin wallets understand.

            nico = BitcoinAddress.Create("15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe");
            Console.WriteLine(nico.ToColoredAddress());
            //Output:
            //akFqRqfdmAaXfPDmvQZVpcAQnQZmqrx4gcZ


            //Now, you can take a look on an Open Asset compatible wallet like Coinprism, and see my asset correctly detected:
            coin = new Coin(
                fromTxHash: new uint256("fa6db7a2e478f3a8a0d1a77456ca5c9fa593e49fd0cf65c7e349e5a4cbe58842"),
                fromOutputIndex: 0,
                amount: Money.Satoshis(2000000),
                scriptPubKey: new Script(Encoders.Hex.DecodeData("76a914356facdac5f5bcae995d13e667bb5864fd1e7d5988ac")));
            BitcoinAssetId assetId = new BitcoinAssetId("AVAVfLSb1KZf9tJzrUVpktjxKUXGxUTD4e");
            ColoredCoin colored = coin.ToColoredCoin(assetId, 10);



            //As I have told you before, the Asset ID is derived from the issuer’s ScriptPubKey, here is how to get it in code:
            var book = BitcoinAddress.Create("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var nicoSecret = new BitcoinSecret("??????????");
            nico = nicoSecret.GetAddress(); //15sYbVpRh6dyWycZMwPdxJWD4xbfxReeHe

            var forFees = new Coin(
                fromTxHash: new uint256("7f296e96ec3525511b836ace0377a9fbb723a47bdfb07c6bc3a6f2a0c23eba26"),
                fromOutputIndex: 0,
                amount: Money.Satoshis(4425000),
                scriptPubKey: new Script(Encoders.Hex.DecodeData("76a914356facdac5f5bcae995d13e667bb5864fd1e7d5988ac")));

            builder = new TransactionBuilder();
            tx = builder
                .AddKeys(nicoSecret)
                .AddCoins(colored, forFees)
                .SendAsset(book, new AssetMoney(assetId, 10))
                .SetChange(nico)
                .SendFees(Money.Coins(0.0001m))
                .BuildTransaction(true);
            Console.WriteLine(tx);
        }
    }
}
