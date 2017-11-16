using System;
using NBitcoin;

namespace PaymentScript
{
    class Program
    {
        static void Main()
        {

            //===========================================================================================
            //Section. ScriptPubKey

            //You might not know that as far as the Blockchain is concerned, there is no such thing as a Bitcoin Address. Internally, the Bitcoin protocol identifies the recipient of Bitcoin by a ScriptPubKey.

            //A ScriptPubKey may look like this:
            //OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
            //It is a short script that explains what conditions must be met to claim ownership of bitcoins. We will go into the types of operations in a ScriptPubKey as we move through the lessons of this book.
            //We are able to generate the ScriptPubKey from the Bitcoin Address. This is a step that all bitcoin clients do to translate the “human friendly” Bitcoin Address to the Blockchain readable address, ScriptPubKey.

            //Picture depiction:
            //Public key -> Public key hash + Network => Bitcoin address -> ScriptPubKey


            //This generates a public key hash directly to have a hardcoded value.
            var publicKeyHash = new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");
            Console.WriteLine(publicKeyHash);
            //Output:
            //14836dbe7f38c5ac3d49e8d790af808a4ee9edcf



            //Generates a bitcoin address for the specific network.
            var bitcoinAddressForTestNet = publicKeyHash.GetAddress(Network.TestNet);
            var bitcoinAddressForMainNet = publicKeyHash.GetAddress(Network.Main);

            Console.WriteLine($"bitcoinAddressForTestNet {bitcoinAddressForTestNet}");
            //Output:
            //bitcoinAddressForTestNet mhPRFiUkjYhxd8xAD5feYYpqV7ZVaCKAcT

            Console.WriteLine($"bitcoinAddressForMainNet {bitcoinAddressForMainNet}");
            //Output:
            //bitcoinAddressForMainNet 12sTxfPmvXGhr2UYVWhGidcWd7xne8mp6T

            //Generate a ScriptPubKey from a Bitcoin address.
            Console.WriteLine(bitcoinAddressForMainNet.ScriptPubKey);
            //Output:
            //OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
            Console.WriteLine(bitcoinAddressForTestNet.ScriptPubKey);
            //Output:
            //OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG



            //Notice the ScriptPubKey for testnet and mainnet address is the same ?
            //Notice the ScriptPubKey contains the hash of the public key?
            //We will not go into the details yet, but note that the ScriptPubKey appears to have nothing to do with the Bitcoin Address, but it does show the hash of the public key.
            //Bitcoin Addresses are composed of a version byte which identifies the network where to use the address and the hash of a public key.So we can go backwards and generate a bitcoin address from the ScriptPubKey and the network identifier.


            //Generate a ScriptPubKey from publicKeyHash.
            var paymentScript = publicKeyHash.ScriptPubKey;
            //Get a Bitcoin address by specifying a network type on ScriptPubKey.
            var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main);
            Console.WriteLine(bitcoinAddressForMainNet == sameMainNetAddress);
            //Output:
            //True

            //It is also possible to retrieve the public key hash from the ScriptPubKey and generate a Bitcoin Address by specifying a network type on a public key hash:
            var samePublicKeyHash = (KeyId)paymentScript.GetDestination();
            Console.WriteLine(publicKeyHash == samePublicKeyHash); 
            //Output:
            //True

            //Get a Bitcoin address by publickeyHash and network identifier.
            var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main);
            Console.WriteLine(bitcoinAddressForMainNet == sameMainNetAddress2); 
            //Output:
            //True


            //Note: A ScriptPubKey does not necessarily contain the hashed public key(s) permitted to spend the bitcoin.
            //So now you understand the relationship between a Private Key, a Public Key, a Public Key Hash, a Bitcoin Address and a ScriptPubKey.
            //In the remainder of this book, we will exclusively use a ScriptPubKey. A Bitcoin Address is only for a user interface concept for representing a recipient. Data which is used in practical Blockchain, for representing a recipient, by being inserted into a TxOut of a transaction is a ScriptPubKey.

            Console.ReadLine();
        }
    }
}
