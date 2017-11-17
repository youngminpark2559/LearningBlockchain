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
            //Section. Proof of ownership as an authentication method
            
            //[2016.05.02] My name is Craig Wright and I am about to demonstrate a signing of a message with the public key that is associated with the first transaction ever done in Bitcoin.
            SignAsCraigWright();
            VerifySatoshi();
            VerifyDorier();

            //BONUS: Get the first Bitcoin Address
            Console.WriteLine(GetFirstBitcoinAddressEver());
            //Output:
            //1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa
        }


        static void SignAsCraigWright()
        {
            RandomUtils.Random = new UnsecureRandom();

            var bitcoinPrivateKey = new BitcoinSecret("KzgjNRhcJ3HRjxVdFhv14BrYUKrYBzdoxQyR2iJBHG9SNGGgbmtC");
            var message = "I am Craig Wright";
            Console.WriteLine($"bitcoinPrivateKey.PrivateKey: {bitcoinPrivateKey.PrivateKey}");
            string signature = bitcoinPrivateKey.PrivateKey.SignMessage(message);
            Console.WriteLine($"bitcoinPrivateKey.Network: {bitcoinPrivateKey.Network}");
            Console.WriteLine($"signature: {signature}");
            //Output:
            //IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=

            //Was that so hard?
            //You may remember Craig Wright, who really wanted us to believe he is Satoshi Nakamoto.
            //He had successfully convinced a handful of influential Bitcoin people and journalists with some social engineering.
            //Fortunately digital signatures do not work that way.
        }

        static void VerifySatoshi()
        {
            //Let's quickly find the first ever bitcoin address on the Internet, associated with the genesis block:
            //1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa 
            //and verify his claim:

            var message = "I am Craig Wright";
            var signature = "IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=";

            var address = new BitcoinPubKeyAddress("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");
            //Verify the message and signature by comparing ones located in the genesis block.
            bool isCraigWrightSatoshi = address.VerifyMessage(message, signature);

            Console.WriteLine($"isCraigWrightSatoshi: {isCraigWrightSatoshi}");


            //SPOILER ALERT! The bool will be false.
            //This is how you prove you are the owner of an address without moving coins:
        }

        static void VerifyDorier()
        {
            //This constitutes proof that Nicolas Dorier owns the private key of the book.
            //Address:
            //1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB
            //Message:
            //Nicolas Dorier Book Funding Address
            //Signature:
            //H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=+

            //Exercise: Verify that Nicolas sensei is not lying!

            var address = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var message = "Nicolas Dorier Book Funding Address";
            var signature = "H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=";
            
            Console.WriteLine($"address.VerifyMessage(message, signature): {address.VerifyMessage(message, signature)}");
        }

        //This is the process of getting the first bitcoin address which was made by Satoshi Nakamoto. 
        //The process is that 
        //1.Get the first block
        //var genesisBlock = Network.Main.GetGenesis() -> 
        //2.From the first block, get the first transaction
        //var firstTransactionEver = genesisBlock.Transactions.First() -> 
        //3.From the first transaction, get the first output
        //var firstOutputEver = firstTransactionEver.Outputs.First() -> 
        //4.From the first output, get the first ScriptPubKey
        //var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey -> 
        //5.From the first ScriptPubKey, get the first destination of the first ScriptPubKey and it will be the first public key 
        //var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First() -> 
        //6.From the first public key, get the first address.
        //var firstBitcoinAddressEver = firstPubKeyEver.GetAddress(Network.Main)
        static string GetFirstBitcoinAddressEver()
        {
            
            //You probably know the Blockchain is a chain of blocks.
            //And all the way back to the first block ever, it's called genesis block.
            //Here is how you can get it: 
            
            var genesisBlock = Network.Main.GetGenesis();

            
            //You probably also know a block is made up of transactions.
            //Here is how you can get the first transaction ever:
            var firstTransactionEver = genesisBlock.Transactions.First();

            
            //You might not know that a transaction can have multiple outputs (and inputs).
            //Here is how you can get the first output ever:
            var firstOutputEver = firstTransactionEver.Outputs.First();

            
            //You usually see a destination of an output as a bitcoin address.
            //But the Bitcoin network doesn't know bitcoin addresses.
            //A bitcoin address is only human-readable format data.
            //We newly generate a data from a bitcoin address.
            //And we call it a ScriptPubKey which is blockchain-readable playing a role like a bitcoin ddress for humans. 
            //A ScriptPubKey looks something like this:
            //OP_DUP OP_HASH160 62e907b15cbf27d5425399ebf6f0fb50ebb88f18 OP_EQUALVERIFY OP_CHECKSIG 
            //Let's get the ScriptPubKey of the first output ever:  
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
