using System;
using NBitcoin;


namespace PrivateKey
{
    class Program
    {
        static void Main()
        {
            //==========================================================================================
            //Chapter4. Private key

            //Private keys are often represented in Base58Check encoding scheme. And private key represented in Base58Check is especially called a Bitcoin Secret.
            //Bitcoin secret is also known as a Wallet Import Format or simply WIF, because Bitcoin secret is usually used within a Bitcoin wallet which is a kind of an UI tool, along with a Bitcoin address. Usually "Bitcoin secret" is used for signing a signature for the coin and proof of ownership for the coin, and "Bitcoin address" is used for representing a recipient for the coin, in the UI layer, for creating a transaction.




            RandomUtils.Random = new UnsecureRandom();


            //Generate one random private key.
            //Key privateKey = new Key();

            //Create a private key by the hardcode value to get consistent result from every execution.
            BitcoinSecret bitcoinSecretForThisExample = new BitcoinSecret("L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo", Network.Main);
            Key privateKey = bitcoinSecretForThisExample.PrivateKey;
            

            //Generate a Bitcoin secret for the MainNet, which is nothing but a private key represented in Base58Check binary-to-text encoding scheme.
            BitcoinSecret privateKeyForMainNet = privateKey.GetBitcoinSecret(Network.Main);
            //Generate a Bitcoin secret for the TestNet, which is nothing but a private key represented in Base58Check binary-to-text encoding scheme.
            BitcoinSecret privateKeyForTestNet = privateKey.GetBitcoinSecret(Network.TestNet);

           
            Console.WriteLine($"privateKeyForMainNet: {privateKeyForMainNet}");
            //Output:
            //L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo
            Console.WriteLine($"privateKeyForTestNet: {privateKeyForTestNet}");
            //Output:
            //cVaZH9dSeHPxuUi7HAhtdqpyfZSgdLzEXgmRL7obD1zdNmxcW9aL










            //You can also generate a private key by invoking GetWif() on the private key by additionally specifying a network identifier.
            //Note that we're using the same private key generated from above.
            BitcoinSecret privateKeyByGetWifMethod = privateKey.GetWif(Network.Main);
            Console.WriteLine(privateKeyByGetWifMethod);
            //Output:
            //L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn

            bool wifIsPrivateKey = privateKeyForMainNet == privateKey.GetWif(Network.Main);
            Console.WriteLine(wifIsPrivateKey);
            //Output:
            //True


            //Note that it is easy to go from a Bitcoin secret to a private key. Recall that a Bitcoin secret is nothing but a private key just represented in Base58Check from a private key, which is often used in UI layer in Bitcoin system such as via wallet software. In the NBitcoin, when you instantiate a key object by a "new Key()", under the hood, you also invoke a secure PRNG to generate a random key data for a private key which will be stored in the key object. On the other hand, it is impossible to go from a Bitcoin address to a public key because the Bitcoin address is generated from a hash of the public key(and + network identifier), not the public key itself.

            //Process this information by examining the similarities between these two codeblocks:

            //Get the Bitcoin secret by invoking GetWif() on the private key with passing additionally the network identifier.
            BitcoinSecret bitcoinSecretByGetWif = privateKey.GetWif(Network.Main);
            //Get the Bitcoin secret by invoking GetBitcoinSecret() on the private key with passing additionally the network identifier.
            BitcoinSecret bitcoinSecretByGetBitcoinSecret = privateKey.GetBitcoinSecret(Network.Main);

            //Get the private key from the Bitcoin secret.
            var privateKeyFromBsByGetWif = bitcoinSecretByGetWif.PrivateKey;
            var privateKeyFromBsByGetBitcoinSecret = bitcoinSecretByGetBitcoinSecret.PrivateKey;

            

            Console.WriteLine($"privateKeyFromBsByGetWif: {privateKeyFromBsByGetWif.ToString(Network.Main)}");
            Console.WriteLine($"privateKeyFromBsByGetBitcoinSecret: {privateKeyFromBsByGetBitcoinSecret.ToString(Network.Main)}");
            //L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo
            //L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo
            Console.WriteLine(privateKey==privateKeyFromBsByGetWif);
            //Output:
            //True





            PubKey publicKey = privateKey.PubKey;
            BitcoinPubKeyAddress bitcoinAddress = publicKey.GetAddress(Network.Main);
            Console.WriteLine($"bitcoinAddress: {bitcoinAddress}");
            //Output:
            //1ErHHqvzRT6WgVzX5J7tin5LaDGev2UobP

            //But it is impossible to get a public key from a Bitcoin address.
            //PubKey publicKeyFromBitcoinAddress = bitcoinAddress.ItIsNotPossible;

            Console.ReadLine();
        }
    }
}
