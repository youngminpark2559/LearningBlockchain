using System;
using System.Linq;
using NBitcoin;
using NBitcoin.Stealth;



namespace _3._1TransactionBuilder
{
    class Program
    {
        static void Main()
        {
            //===========================================================================================
            //Chapter8. Using the TransactionBuilder

            //You have seen how the TransactionBuilder works when you have signed your first P2SH and multi-sig transaction.
            //We will see how you can harness its full power, for signing more complicated transactions.
            //With the TransactionBuilder you can:
            //1.Spend any
            //P2PK, P2PKH,
            //multi-sig,
            //P2WPK, P2WSH.
            //2.Spend any P2SH on the previous redeem script.
            //3.Spend Stealth Coin(DarkWallet).
            //4.Issue and transfer Colored Coins(open asset, following chapter).
            //5.Combine partially signed transactions.
            //6.Estimate the final size of an unsigned transaction and its fees.
            //7.Verify if a transaction is fully signed.

            //The goal of the TransactionBuilder is to take Coins and Keys as input, and return back a signed or partially signed transaction.
            //Picture depiction:
            //Coins -> Signed transaction(or it could be a signed transactions) <-Keys.


            //The TransactionBuilder will figure out what Coin to use and what to sign by itself.
            //Examine TransactionBuilder class containing lots of properties and methods.


            //The usage of the builder is done in four steps:
            //1.You gather the Coins that spent,
            //2.You gather the Keys that you own,
            //3.You enumerate how much Money you want to send to where scriptPubKey indicates,
            //4.You build and sign the transaction,
            //5.Optional: you give the transaction to somebody else, then he will sign or continue to build it.


            //Now let’s gather some Coins. 
            //For that, let us create a fake transaction with some funds on it.
            //Let’s say that the transaction has a P2PKH, P2PK, and multi-sig coin of Bob and Alice.
            RandomUtils.Random = new UnsecureRandom();


            //Private key generator.
            Key privateKeyGenerator = new Key();
            BitcoinSecret bitcoinSecretFromPrivateKeyGenerator = privateKeyGenerator.GetBitcoinSecret(Network.Main);
            Key privateKeyFromBitcoinSecret = bitcoinSecretFromPrivateKeyGenerator.PrivateKey;
            Console.WriteLine($"privateKeyFromBitcoinSecret.ToString(Network.Main): {privateKeyFromBitcoinSecret.ToString(Network.Main)}");
            //L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo is for Alice
            //KxMrK5EJeUZ1z3Jyo2zPkurRVtYFefab4WQitV5CyjKApHsWfWg9 is for Bob
            //KyStsAHgSehHvewS5YfGwhQGfEWYd8qY2XZg6q2M6TqaM8Q8rayg is for Satoshi
            //L2f9Ntm8UUeTLZFv25oZ8WoRW8kAofUjdUdtCq9axCp1hZrsLZja is for Nico

            BitcoinSecret bitcoinSecretForAlice = new BitcoinSecret("L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo", Network.Main);
            BitcoinSecret bitcoinSecretForBob = new BitcoinSecret("KxMrK5EJeUZ1z3Jyo2zPkurRVtYFefab4WQitV5CyjKApHsWfWg9", Network.Main);
            BitcoinSecret bitcoinSecretForSatoshi = new BitcoinSecret("KyStsAHgSehHvewS5YfGwhQGfEWYd8qY2XZg6q2M6TqaM8Q8rayg", Network.Main);
            BitcoinSecret bitcoinSecretForScanKey = new BitcoinSecret("L2f9Ntm8UUeTLZFv25oZ8WoRW8kAofUjdUdtCq9axCp1hZrsLZja", Network.Main);


            Key bobPrivateKey = bitcoinSecretForAlice.PrivateKey;
            Key alicePrivateKey = bitcoinSecretForBob.PrivateKey;
            Key satoshiPrivateKey = bitcoinSecretForSatoshi.PrivateKey;
            Key privateKeyForScanKey = bitcoinSecretForScanKey.PrivateKey;
            



            Script scriptPubKeyOfBobAlice =
                PayToMultiSigTemplate.Instance.GenerateScriptPubKey(2, bobPrivateKey.PubKey, alicePrivateKey.PubKey);

            //This transaction will send money to Bob and Alice.
            //The thing you should notice is that this transaction is added by various types of scriptPubKey, such as P2PK(bobPrivateKey.PubKey), P2PKH(alicePrivateKey.PubKey.Hash), and multi-sig ScriptPubKey(scriptPubKeyOfBobAlice).
            var txGettingCoinForBobAlice = new Transaction();
            txGettingCoinForBobAlice.Outputs.Add(new TxOut(Money.Coins(1m), bobPrivateKey.PubKey)); // P2PK
            txGettingCoinForBobAlice.Outputs.Add(new TxOut(Money.Coins(1m), alicePrivateKey.PubKey.Hash)); // P2PKH
            txGettingCoinForBobAlice.Outputs.Add(new TxOut(Money.Coins(1m), scriptPubKeyOfBobAlice));

            //Now let’s say they want to use the coins of this transaction to pay Satoshi.

            //First they have to get their coins.
            Coin[] coins = txGettingCoinForBobAlice.Outputs.AsCoins().ToArray();

            Coin bobCoin = coins[0];
            Coin aliceCoin = coins[1];
            Coin bobAliceCoin = coins[2];

            //Now let’s say Bob wants to send 0.2 BTC, Alice 0.3 BTC, and they agree to use bobAlice to send 0.5 BTC.
            //Build the transaction by using the features of the TransactionBuilder class.
            var builderForSendingCoinToSatoshi = new TransactionBuilder();
            Transaction txForSpendingCoinToSatoshi = builderForSendingCoinToSatoshi
                    .AddCoins(bobCoin)
                    //PriveteKey of Bob to be used for signing.
                    .AddKeys(bobPrivateKey)
                    .Send(satoshiPrivateKey, Money.Coins(0.2m))
                    .SetChange(bobPrivateKey)
                    .Then()
                    .AddCoins(aliceCoin)
                    .AddKeys(alicePrivateKey)
                    .Send(satoshiPrivateKey, Money.Coins(0.3m))
                    .SetChange(alicePrivateKey)
                    .Then()
                    .AddCoins(bobAliceCoin)
                    .AddKeys(bobPrivateKey, alicePrivateKey)
                    .Send(satoshiPrivateKey, Money.Coins(0.5m))
                    .SetChange(scriptPubKeyOfBobAlice)
                    .SendFees(Money.Coins(0.0001m))
                    .BuildTransaction(sign: true);
            Console.WriteLine(txForSpendingCoinToSatoshi);

            //Then you can verify it is fully signed and ready to send to the network.
            //Verify you did not screw up.
            Console.WriteLine(builderForSendingCoinToSatoshi.Verify(txForSpendingCoinToSatoshi)); // True



            //============================================================================================
            //Do with a ScriptCoin.

            //The nice thing about this model is that it works the same way for P2SH, P2WSH, P2SH(P2WSH), and P2SH(P2PKH) except you need to create ScriptCoin.

            //Illustration:
            //Coin-> ScriptCoin <-RedeemScript.

            var txGettingScriptCoinForBobAlice = new Transaction();
            txGettingScriptCoinForBobAlice.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKeyOfBobAlice.Hash));

            coins = txGettingScriptCoinForBobAlice.Outputs.AsCoins().ToArray();
            ScriptCoin bobAliceScriptCoin = coins[0].ToScriptCoin(scriptPubKeyOfBobAlice);

            //Then the signature:
            var builderForSendingScriptCoinToSatoshi = new TransactionBuilder();
            var txForSendingScriptCoinToSatoshi = builderForSendingScriptCoinToSatoshi
                    .AddCoins(bobAliceScriptCoin)
                    .AddKeys(bobPrivateKey, alicePrivateKey)
                    .Send(satoshiPrivateKey, Money.Coins(0.9m))
                    .SetChange(scriptPubKeyOfBobAlice.Hash)
                    .SendFees(Money.Coins(0.0001m))
                    .BuildTransaction(true);
            Console.WriteLine(builderForSendingScriptCoinToSatoshi.Verify(txForSendingScriptCoinToSatoshi));
            //Output:
            //True


            //============================================================================================
            //Do with a StealthCoin.

            //For Stealth Coin, this is basically the same thing except that, if you remember our introduction on Dark Wallet, you need a ScanKey to see the StealthCoin.

            //Illustration:
            //ScanKey + Transaction + StealthAddress => StealthCoin.

            //Let’s create darkAliceBob stealth address as in previous chapter:

            
            BitcoinStealthAddress bitcoinStealthAddressForBobAlice =
                new BitcoinStealthAddress
                    (
                        scanKey: privateKeyForScanKey.PubKey,
                        pubKeys: new[] { alicePrivateKey.PubKey, bobPrivateKey.PubKey },
                        signatureCount: 2,
                        bitfield: null,
                        network: Network.Main
                    );


            //Let’s say someone sent the coin to this transaction via the darkAliceBob which is a BitcoinStealthAddress:
            var txGettingCoinForBobAliceToBitcoinStealthAddress = new Transaction();
            bitcoinStealthAddressForBobAlice
                .SendTo(txGettingCoinForBobAliceToBitcoinStealthAddress, Money.Coins(1.0m));

            //The scanner will detect the StealthCoin:
            //Get the stealth coin with the scanKey.
            StealthCoin stealthCoin
                = StealthCoin.Find(txGettingCoinForBobAliceToBitcoinStealthAddress, bitcoinStealthAddressForBobAlice, privateKeyForScanKey);

            //And forward it to Bob and Alice, who will sign:
            //Let Bob and Alice sign and spend the coin.
            TransactionBuilder builderForBobAliceToBitcoinStealthAddress = new TransactionBuilder();
            txGettingCoinForBobAliceToBitcoinStealthAddress = builderForBobAliceToBitcoinStealthAddress
                    .AddCoins(stealthCoin)
                    .AddKeys(bobPrivateKey, alicePrivateKey, privateKeyForScanKey)
                    .Send(satoshiPrivateKey, Money.Coins(0.9m))
                    .SetChange(scriptPubKeyOfBobAlice.Hash)
                    .SendFees(Money.Coins(0.0001m))
                    .BuildTransaction(true);
            Console.WriteLine(builderForBobAliceToBitcoinStealthAddress.Verify(txGettingCoinForBobAliceToBitcoinStealthAddress));
            //Output:
            //True

            //Note: You need the scanKey for spending a StealthCoin
        }
    }
}
