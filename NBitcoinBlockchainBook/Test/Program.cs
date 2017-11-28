using NBitcoin;
using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();

            //===========================================================================================
            //Chapter3. ScriptPubKey


            BitcoinSecret bitcoinSecretForThisExample = new BitcoinSecret("L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo", Network.Main);
            Console.WriteLine($"bitcoinSecretForThisExample: {bitcoinSecretForThisExample}");



            Key privateKeyForThisExample = bitcoinSecretForThisExample.PrivateKey;
            Console.WriteLine($"privateKeyForThisExample: {privateKeyForThisExample}");



            var publicKeyForThisExample = privateKeyForThisExample.PubKey;
            Console.WriteLine($"publicKeyForThisExample: {publicKeyForThisExample}");



            var publicKeyHashForThisExample = publicKeyForThisExample.Hash;
            Console.WriteLine($"publicKeyHashForThisExample: {publicKeyHashForThisExample}");




            //For a reference, this is the way of generating a public key directly by passing the public key hash string value (97eb9da945f16139acb552a2de4081eb7f5176d9) which you want to use as the publich key hash.
            //This generates a public key hash directly to have a hardcoded value.
            var publicKeyHashByHardCodedValue = new KeyId("97eb9da945f16139acb552a2de4081eb7f5176d9");
            Console.WriteLine($"publicKeyHashBySimpleWay: {publicKeyHashByHardCodedValue}");




            //Generates a bitcoin address for the specific network.
            var bitcoinAddressForTestNet = publicKeyHashForThisExample.GetAddress(Network.TestNet);
            var bitcoinAddressForMainNet = publicKeyHashForThisExample.GetAddress(Network.Main);

            Console.WriteLine($"bitcoinAddressForTestNet {bitcoinAddressForTestNet}");


            Console.WriteLine($"bitcoinAddressForMainNet {bitcoinAddressForMainNet}");





            //Generate a ScriptPubKey from a Bitcoin address.
            Console.WriteLine(bitcoinAddressForMainNet.ScriptPubKey);

            Console.WriteLine(bitcoinAddressForTestNet.ScriptPubKey);





            //Generate a ScriptPubKey from publicKeyHash.
            var scriptPubKeyForPayment = publicKeyHashForThisExample.ScriptPubKey;
            //Get a Bitcoin address by specifying a network identifier on the ScriptPubKey.
            var bitcoinAddressFromSPKForMainNet = scriptPubKeyForPayment.GetDestinationAddress(Network.Main);
            Console.WriteLine(bitcoinAddressForMainNet == bitcoinAddressFromSPKForMainNet);





            //Get the public key hash from the ScriptPubKey.
            var publicKeyHashFromSPK = (KeyId)scriptPubKeyForPayment.GetDestination();
            Console.WriteLine(publicKeyHashForThisExample == publicKeyHashFromSPK);



            //Get the Bitcoin address by using the publick key hash retrieved from the ScriptPubKey from above code and network identifier.
            var bitcoinAddressGotAgain = new BitcoinPubKeyAddress(publicKeyHashFromSPK, Network.Main);
            Console.WriteLine(bitcoinAddressForMainNet == bitcoinAddressGotAgain);



            Console.ReadLine();
        }
    }
}
