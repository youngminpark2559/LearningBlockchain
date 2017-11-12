using System;
using NBitcoin;

//c Add console project 1.3PrivateKey to examine private key. Private key is represented in Base58Check. Base58Check is called BitcoinSecret or WIF(Wallet Import Format). It's easy to go from BitcoinSecret to private key. However, it's impossible to go from a Bitcoin Address to Public key, because the Bitcoin Address contains a hash of the public key, not the public key itself.

namespace PrivateKey
{
    class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();
            Key privateKey = new Key(); // generate a random private key
            BitcoinSecret mainNetPrivateKey = privateKey.GetBitcoinSecret(Network.Main);  // get our private key for the mainnet
            BitcoinSecret testNetPrivateKey = privateKey.GetBitcoinSecret(Network.TestNet);  // get our private key for the testnet
            Console.WriteLine(mainNetPrivateKey); // L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
            Console.WriteLine(testNetPrivateKey); // cVY5auviDh8LmYUW8AfafseD6p6uFoZrP7GjS3rzAerpRKE9Wmuz

            bool WifIsBitcoinSecret = mainNetPrivateKey == privateKey.GetWif(Network.Main);
            Console.WriteLine(WifIsBitcoinSecret); // True


            BitcoinSecret bitcoinPrivateKey = privateKey.GetWif(Network.Main); // L5B67zvrndS5c71EjkrTJZ99UaoVbMUAK58GKdQUfYCpAa6jypvn
            Key samePrivateKey = bitcoinPrivateKey.PrivateKey;

            PubKey publicKey = privateKey.PubKey;
            BitcoinPubKeyAddress bitcoinPubicKey = publicKey.GetAddress(Network.Main); // 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
            //PubKey samePublicKey = bitcoinPubicKey.ItIsNotPossible;

            Console.ReadLine();
        }
    }
}
