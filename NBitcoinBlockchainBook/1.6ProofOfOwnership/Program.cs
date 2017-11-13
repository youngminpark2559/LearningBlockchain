using System;
using System.Linq;
using NBitcoin;

//c Add code. I verify the ownership of an address without moving coins. And I verify the first address of the bitcoin through the set of process.

namespace ProofOfOwnership
{
    class Program
    {
        static void Main()
        {
            SignAsCraigWright();
            VerifySatoshi();
            VerifyDorier();

            /* BONUS: Get the first Bitcoin Address */
            Console.WriteLine(GetFirstBitcoinAddressEver()); // 1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa
        }


        static void SignAsCraigWright()
        {
            var bitcoinPrivateKey = new BitcoinSecret("KzgjNRhcJ3HRjxVdFhv14BrYUKrYBzdoxQyR2iJBHG9SNGGgbmtC");
            var message = "I am Craig Wright";
            string signature = bitcoinPrivateKey.PrivateKey.SignMessage(message);
            Console.WriteLine(signature); 
            // IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=
        }

        static void VerifySatoshi()
        {
            var message = "I am Craig Wright";
            var signature = "IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=";

            //The first ever bitcoin address which is associated with the "genesis block", 1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa, and verify his claim by this address.
            var address = new BitcoinPubKeyAddress("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");
            //Verify the message and signature by comparing ones resided in address.
            bool isCraigWrightSatoshi = address.VerifyMessage(message, signature);

            Console.WriteLine("Is Craig Wright Satoshi? " + isCraigWrightSatoshi);

            //This is how I prove I'm the owner of an address without moving coins.
        }

        static void VerifyDorier()
        {
            var address = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var message = "Nicolas Dorier Book Funding Address";
            var signature = "H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=";
            //This will return true.
            Console.WriteLine($"Compare message and signature with the ones resided in the address, and verify : {address.VerifyMessage(message, signature)}");
        }

        //This is the process of getting the first bitcoin address which maybe was made by Satoshi nakamoto. The process is that 
        //1.get the first block
        //var genesisBlock = Network.Main.GetGenesis() -> 
        //2.from the first block, get the first transaction
        //var firstTransactionEver = genesisBlock.Transactions.First() -> 
        //3.from the first transaction, get the first output
        //var firstOutputEver = firstTransactionEver.Outputs.First() -> 
        //4.from the first output, get the first ScriptPubKey
        //var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey -> 
        //5.from the first ScriptPubKey, get the first destination of the first ScriptPubKey and it will be the first public key 
        //var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First() -> 
        //from the first public key, get the first address.
        //var firstBitcoinAddressEver = firstPubKeyEver.GetAddress(Network.Main)
        static string GetFirstBitcoinAddressEver()
        {
            /* 
             * You probably know The Blockchain is a chain of blocks,
             * All the way back to the first block ever, called genesis.
             * Here is how you can get it: 
             */
            var genesisBlock = Network.Main.GetGenesis();

            /* 
             * You probably also know a block is made up of transactions.
             * Here is how you can get the first transaction ever:
             */
            var firstTransactionEver = genesisBlock.Transactions.First();

            /* 
             * You might not know that a transaction can have multiple outputs (and inputs).
             * Here is how you can get the first output ever:
             */
            var firstOutputEver = firstTransactionEver.Outputs.First();

            /* 
             * You usually see a destination of an output as a bitcoin address             * 
             * But the Bitcoin network doesn't know addresses, it knows ScriptPubKeys
             * A ScriptPubKey looks something like this:
             * OP_DUP OP_HASH160 62e907b15cbf27d5425399ebf6f0fb50ebb88f18 OP_EQUALVERIFY OP_CHECKSIG 
             * Let's get the ScriptPubKey of the first output ever:  
             */
            var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey;

            /*
             * Actually your wallet software is what decodes addresses into ScriptPubKeys.
             * Or decodes ScriptPubKeys to addresses (when it is possible).
             * Here is how it does it:
             */

            /* 
             * In early times a ScriptPubKey contained one public key.
             * Here is the first ScriptPubKey ever:
             * 04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG
             * From a public key you can derive a bitcoin address.
             * So in order to get the first address ever, we can get the first public key ever:
             */


            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();

            /*
             * You can get a bitcoin address from a public key and with the network identifier:
             */
            var firstBitcoinAddressEver = firstPubKeyEver.GetAddress(Network.Main);

            return firstBitcoinAddressEver.ToString();
        }
    }
}
