using System;
using NBitcoin;

//c Add console project 1.3PrivateKey to examine private key. Private key is represented in Base58Check. Base58Check is called BitcoinSecret or WIF(Wallet Import Format). It's easy to go from BitcoinSecret to private key. However, it's impossible to go from a Bitcoin Address to Public key, because the Bitcoin Address contains a hash of the public key, not the public key itself.

namespace PrivateKey
{
    class Program
    {
        static void Main()
        {
            //==========================================================================================
            //Section. Private key

            //Private keys are often represented in Base58Check encoding scheme. And private key represented in Base58Check is called a Bitcoin Secret.
            //Bitcoin secret is also known as a Wallet Import Format or simply WIF, because Bitcoin secret is usually used within a Bitcoin wallet which is a kind of an UI tool, along with a Bitcoin address. Usually Bitcoin secret is used for signing a signature and proof of ownership for the coin, and Bitcoin address is used for representing a recipient for the coin, in the UI for Bitcoin transaction.




                       RandomUtils.Random = new UnsecureRandom();
            //Generate a random key object.
            Key keyObj = new Key();
            //Generate a Bitcoin secret for a MainNet, which is nothing but a private key represented in Base58Check.
            BitcoinSecret privateKeyForMainNet = keyObj.GetBitcoinSecret(Network.Main);
            //Generate a Bitcoin secret for a TestNet, which is nothing but a private key represented in Base58Check.
            BitcoinSecret privateKeyForTestNet = keyObj.GetBitcoinSecret(Network.TestNet);

            Console.WriteLine($"privateKeyForMainNet: {privateKeyForMainNet}");
            //Output:
            //L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
            Console.WriteLine($"privateKeyForTestNet: {privateKeyForTestNet}");
            //Output:
            //cVY5auviDh8LmYUW8AfafseD6p6uFoZrP7GjS3rzAerpRKE9Wmuz

            //You can also generate a private key by GetWif() method on a privateKey by specifying a network type.
            //Note that we're using the same privateKey generated from above.
            BitcoinSecret privateKeyByGetWifMethod = keyObj.GetWif(Network.Main);
            Console.WriteLine(privateKeyByGetWifMethod);
            //Output:
            //L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn

            bool wifIsPrivateKey = privateKeyForMainNet == keyObj.GetWif(Network.Main);
            Console.WriteLine(wifIsPrivateKey);
            //Output:
            //True


            //            Note that it is easy to go from BitcoinSecret to private Key.On the other hand, it is impossible to go from a Bitcoin Address to Public Key because the Bitcoin Address contains a hash of the Public Key, not the Public Key itself.
            //Process this information by examining the similarities between these two codeblocks:


            var keyObj1 = new Key();
            BitcoinSecret bitcoinSecretByGetWif = keyObj1.GetWif(Network.Main);
            BitcoinSecret bitcoinSecretByGetBitcoinSecret = keyObj1.GetBitcoinSecret(Network.Main);

            var pk1 = bitcoinSecretByGetWif.PrivateKey;
            var pk2 = bitcoinSecretByGetBitcoinSecret.PrivateKey;

            Console.WriteLine("===========");

            Console.WriteLine(bitcoinSecretByGetWif);
            Console.WriteLine(bitcoinSecretByGetBitcoinSecret);
            Console.WriteLine(pk1);
            Console.WriteLine(pk2);
            //L3syD8Z3biLy9Qb2MfUGZNTe3yfFMYf9r8oRXVJPng4GM1My9jsv
            //L3syD8Z3biLy9Qb2MfUGZNTe3yfFMYf9r8oRXVJPng4GM1My9jsv
            //NBitcoin.Key
            //NBitcoin.Key



            // L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
            //Key samePrivateKey = bitcoinPrivateKey.PrivateKey;

            PubKey publicKey = keyObj.PubKey;
            BitcoinPubKeyAddress bitcoinPubicKey = publicKey.GetAddress(Network.Main); // 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
            //PubKey samePublicKey = bitcoinPubicKey.ItIsNotPossible;

            Console.ReadLine();
        }
    }
}
