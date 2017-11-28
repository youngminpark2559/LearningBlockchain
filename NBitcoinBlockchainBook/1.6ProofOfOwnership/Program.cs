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
            //========================================================================================
            //Chapter8. Proof of an ownership as an authentication method

            SignAsCraigWright();
            VerifySatoshi();
            VerifyDorier();

            //BONUS: Get the first Bitcoin address.
            Console.WriteLine(GetFirstBitcoinAddressEver());
        }


        static void SignAsCraigWright()
        {
            RandomUtils.Random = new UnsecureRandom();

            var bitcoinSecretOfCraig = new BitcoinSecret("KzgjNRhcJ3HRjxVdFhv14BrYUKrYBzdoxQyR2iJBHG9SNGGgbmtC");
            var message = "I am Craig Wright";
            Console.WriteLine($"bitcoinSecretOfCraig.PrivateKey: {bitcoinSecretOfCraig.PrivateKey}");
            string signature = bitcoinSecretOfCraig.PrivateKey.SignMessage(message);
            Console.WriteLine($"signature: {signature}");
            Console.WriteLine($"bitcoinPrivateKey.Network: {bitcoinSecretOfCraig.Network}");
        }

        static void VerifySatoshi()
        {
            var messageWrittenByCraig = "I am Craig Wright";
            var signatureByCraig = "IN5v9+3HGW1q71OqQ1boSZTm0/DCiMpI8E4JB1nD67TCbIVMRk/e3KrTT9GvOuu3NGN0w8R2lWOV2cxnBp+Of8c=";

            var bitcoinAddressOfTheFirstEver = new BitcoinPubKeyAddress("1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa");

            bool isCraigWrightSatoshi = bitcoinAddressOfTheFirstEver.VerifyMessage(messageWrittenByCraig, signatureByCraig);

            Console.WriteLine($"isCraigWrightSatoshi: {isCraigWrightSatoshi}");
        }

        static void VerifyDorier()
        {
            //Exercise: Verify that Nicolas sensei is not lying!
            var addressOfTheBook = new BitcoinPubKeyAddress("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var messageWrittenByNicolas = "Nicolas Dorier Book Funding Address";
            var signatureByNicolas = "H1jiXPzun3rXi0N9v9R5fAWrfEae9WPmlL5DJBj1eTStSvpKdRR8Io6/uT9tGH/3OnzG6ym5yytuWoA9ahkC3dQ=";

            Console.WriteLine($"address.VerifyMessage(message, signature): {addressOfTheBook.VerifyMessage(messageWrittenByNicolas, signatureByNicolas)}");
        }


        //This is the process of getting the first bitcoin address which was made by Satoshi Nakamoto. 
        //The process is like that: 
        //1.Get the first block.
        //var genesisBlock = Network.Main.GetGenesis() -> 
        //2.From the first block, get the first transaction.
        //var firstTransactionEver = genesisBlock.Transactions.First() -> 
        //3.From the first transaction, get the first output.
        //var firstOutputEver = firstTransactionEver.Outputs.First() -> 
        //4.From the first output, get the first ScriptPubKey.
        //var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey -> 
        //5.From the first ScriptPubKey, get the first destination of the first ScriptPubKey and it will be the first public key.
        //var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First() -> 
        //6.From the first public key, get the first bitcoin address.
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

            //Actually your wallet software is what decodes addresses into ScriptPubKey or decodes ScriptPubKeys to addresses (when it is possible).
            //Here is how it does it:


            //In early times a ScriptPubKey contained one public key.
            //Here is the first ScriptPubKey ever:
            //04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG
            //From a public key, you can derive a bitcoin address.
            //So in order to get the first address ever, we can get the first public key ever:
            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();

            //You can get a bitcoin address from a public key and with the network identifier:
            var firstBitcoinAddressEver = firstPubKeyEver.GetAddress(Network.Main);

            return firstBitcoinAddressEver.ToString();
        }
    }
}
