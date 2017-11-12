using System;
using NBitcoin;

namespace PaymentScript
{
    class Program
    {
        static void Main()
        {
            //This generates hashed publickey directly to have a hardcoded value.
            var publicKeyHash = new KeyId("14836dbe7f38c5ac3d49e8d790af808a4ee9edcf");

            //Generates a public address for testnet and mainnet.
            var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);
            var mainNetAddress = publicKeyHash.GetAddress(Network.Main);

            Console.WriteLine(testNetAddress);
            Console.WriteLine(mainNetAddress);

            Console.WriteLine(mainNetAddress.ScriptPubKey);
            //Output:
            //OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG
            Console.WriteLine(testNetAddress.ScriptPubKey);
            //Output:
            //OP_DUP OP_HASH160 14836dbe7f38c5ac3d49e8d790af808a4ee9edcf OP_EQUALVERIFY OP_CHECKSIG

            //Since Bitcoin addresses are composed of a version byte which identifies the network where to use the address(to the mainnet or testnet.) and bytes of hash of the publickey, we can go backwards and generate a Bitcoin address from the ScriptPubKey and the network identifier.
            //Get ScriptPubKey directly from publichkeyHash.
            var paymentScript = publicKeyHash.ScriptPubKey;
            //From ScriptPubKey, by specifying network identifier, get Bitcoin address.
            var sameMainNetAddress = paymentScript.GetDestinationAddress(Network.Main);
            Console.WriteLine(mainNetAddress == sameMainNetAddress); // True

            //Get key id from ScriptPubKey and cast it to the type KeyId(publickeyHash)
            var samePublicKeyHash = (KeyId)paymentScript.GetDestination();
            Console.WriteLine(publicKeyHash == samePublicKeyHash); // True
            
            //Get a Bitcoin address by publickeyHash and network identifier.
            var sameMainNetAddress2 = new BitcoinPubKeyAddress(samePublicKeyHash, Network.Main);
            Console.WriteLine(mainNetAddress == sameMainNetAddress2); // True
        }
    }
}
