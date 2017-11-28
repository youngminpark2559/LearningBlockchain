using System;
using NBitcoin;

namespace PaymentScript
{
    class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();

            //===========================================================================================
            //Section. ScriptPubKey

            //You might not know that, as far as the Blockchain is concerned, there is no such thing as a Bitcoin Address. Internally, the Bitcoin protocol identifies the recipient of Bitcoin by a ScriptPubKey.

            //A ScriptPubKey may look like this:
            //OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
            //It is a short script that explains what conditions must be met to claim ownership of bitcoins. We will go into the types of operations in a ScriptPubKey as we move through the lessons of this book.
            //We are able to generate the ScriptPubKey from the Bitcoin Address. This is a step that all bitcoin clients do to translate the “human friendly” Bitcoin Address to the Blockchain readable address, ScriptPubKey.

            //Illustration:
            //Public key -> Public key hash + Network => Bitcoin address -> ScriptPubKey


            BitcoinSecret bitcoinSecretForThisExample = new BitcoinSecret("L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo", Network.Main);
            Console.WriteLine($"bitcoinSecretForThisExample: {bitcoinSecretForThisExample}");
            //Output:
            //L5DZpEdbDDhhk3EqtktmGXKv3L9GxttYTecxDhM5huLd82qd9uvo

            Key privateKeyForThisExample = bitcoinSecretForThisExample.PrivateKey;
            Console.WriteLine($"privateKeyForThisExample: {privateKeyForThisExample}");
            //Output:
            //NBitcoin.Key

            var publicKeyForThisExample = privateKeyForThisExample.PubKey;
            Console.WriteLine($"publicKeyForThisExample: {publicKeyForThisExample}");
            //Output:
            //03575d9c3bec8dd22fb01aa73e5e8a34b3e305d797df29b9f835e2a69815f085ee

            var publicKeyHashForThisExample = publicKeyForThisExample.Hash;
            Console.WriteLine($"publicKeyHashForThisExample: {publicKeyHashForThisExample}");
            //Output:
            //97eb9da945f16139acb552a2de4081eb7f5176d9




            //For a reference, this is the way of generating a public key directly by passing the public key hash string value (97eb9da945f16139acb552a2de4081eb7f5176d9) which you want to use as the publich key hash.
            //This generates a public key hash directly to have a hardcoded value.
            var publicKeyHashByHardCodedValue = new KeyId("97eb9da945f16139acb552a2de4081eb7f5176d9");
            Console.WriteLine($"publicKeyHashBySimpleWay: {publicKeyHashByHardCodedValue}");
            //Output:
            //97eb9da945f16139acb552a2de4081eb7f5176d9



            //Generates a bitcoin address for the specific network.
            var bitcoinAddressForTestNet = publicKeyHashForThisExample.GetAddress(Network.TestNet);
            var bitcoinAddressForMainNet = publicKeyHashForThisExample.GetAddress(Network.Main);

            Console.WriteLine($"bitcoinAddressForTestNet {bitcoinAddressForTestNet}");
            //Output:
            //muNEau1yEUXmTcU8ns6GYhHfSCsMjseuNB

            Console.WriteLine($"bitcoinAddressForMainNet {bitcoinAddressForMainNet}");
            //Output:
            //1ErHHqvzRT6WgVzX5J7tin5LaDGev2UobP



            




            //Generate a ScriptPubKey from a Bitcoin address.
            Console.WriteLine(bitcoinAddressForMainNet.ScriptPubKey);
            //Output:
            //OP_DUP OP_HASH160 97eb9da945f16139acb552a2de4081eb7f5176d9 OP_EQUALVERIFY OP_CHECKSIG
            Console.WriteLine(bitcoinAddressForTestNet.ScriptPubKey);
            //Output:
            //OP_DUP OP_HASH160 97eb9da945f16139acb552a2de4081eb7f5176d9 OP_EQUALVERIFY OP_CHECKSIG





            //--


            //Can you notice the ScriptPubKeys (bitcoinAddressForMainNet.ScriptPubKey and bitcoinAddressForTestNet.ScriptPubKey) generated from each Bitcoin address for the TestNet and the MainNet are exactly identical?
            //Can you notice the ScriptPubKey contains the hash of the public key hash (97eb9da945f16139acb552a2de4081eb7f5176d9)?
            //We will not go into the details yet. However, note that the ScriptPubKey appears to have nothing to do with the Bitcoin address, just with showing the public key hash value from a part of the entire ScriptPubKey.

            //Bitcoin addresse is composed of a "version byte" which identifies the network type where for the bitcoin address to being used and the "hash of a public key". So we can go backwards and generate a bitcoin address from the ScriptPubKey and the network identifier.


            //Generate a ScriptPubKey from publicKeyHash.
            var scriptPubKeyForPayment = publicKeyHashForThisExample.ScriptPubKey;
            //Get a Bitcoin address by specifying a network identifier on the ScriptPubKey.
            var bitcoinAddressFromSPKForMainNet = scriptPubKeyForPayment.GetDestinationAddress(Network.Main);
            Console.WriteLine(bitcoinAddressForMainNet == bitcoinAddressFromSPKForMainNet);
            //Output:
            //True


            //It is also possible to retrieve the public key hash from the ScriptPubKey and also generate a Bitcoin address by specifying a network identifier on the public key hash generated in this way:

            //Get the public key hash from the ScriptPubKey.
            var publicKeyHashFromSPK = (KeyId)scriptPubKeyForPayment.GetDestination();
            Console.WriteLine(publicKeyHashForThisExample == publicKeyHashFromSPK); 
            //Output:
            //True

            //Get the Bitcoin address by using the publick key hash retrieved from the ScriptPubKey from above code and network identifier.
            var bitcoinAddressGotAgain = new BitcoinPubKeyAddress(publicKeyHashFromSPK, Network.Main);
            Console.WriteLine(bitcoinAddressForMainNet == bitcoinAddressGotAgain);
            //Output:
            //True


            //Note: A ScriptPubKey does not necessarily contain the hashed public key(s) permitted to spend the bitcoin like this:
            //OP_DUP OP_HASH160 <Omitted public key hash value> OP_EQUALVERIFY OP_CHECKSIG


            //So now you understand the relationship between a Private Key, a Public Key, a Public Key Hash, a Bitcoin Address and a ScriptPubKey.
            //In the remainder of this book, we will exclusively use a ScriptPubKey. 
            //Again, a Bitcoin address representing a recipient is just the address human-readable for the UI layer like wallet software. Address representing a recipient, which is blockchain system readable is, the ScriptPubKey which is used in a TxOut of a transaction.
            
            Console.ReadLine();
        }
    }
}
