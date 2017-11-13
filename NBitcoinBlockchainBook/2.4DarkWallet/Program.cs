using System;
using NBitcoin;
using NBitcoin.Stealth;

//Dark wallet is mostly nothing to do with dark. Dark wallet is a practical solution that fixes 2 problems which I had, 1.Delegating key/address generation to an untrusted peer, 2.Prevent outdated backups.
//But Dark wallet has a bonus killer feature.
//Let's suppose that I have to share only one address with the world, let's call this address StealthAddress, without leaking any privacy.
//And let's remind that if I share one BitcoinAddress with everybody, then all can see my balance by consulting the blockchain. That's not the case with a StealthAddress.
//It is a real shame that it was labeled as dark since it solves partially the important problem of privacy leaking caused by the pseudo-anonymity of Bitcoin. A better name would have been: One Address.
//In Dark Wallet terminology, here are the different actors: The payer, the receiver, and the scanner.
//The payer knows the StealthAddress of the receiver.
//The receiver knows the spend key, a secret which will allow him to spend the coins which he receives from such a transaction that the payer sends coins to the receiver.
//The scanner knows the Scan Key, a secret which allows him to detect the transactions which belong to the Receiver.
//The rest is operational details.
//Underneath, this StealthAddress is composed of one or several Spen PubKey(when it's several it's for multi sig), and one Scan PubKey.




//The payer will take your StealthAddress, and generate a temporary key called Ephem Key and then convert Ephem Key to Ephem PubKey, and generate a Stealth Pub Key with StealthAddress and Ephem PubKey. And from the Stealth PubKey, the payer will generate Bitcoin Address to which the payment will be made.


//Then, they will package the Ephem PubKey in a Stealth Metadata object which is embedded in the OP_RETURN of the transaction(as I did for the first challenge).
//They will also add the output to the generated bitcoin address which is the address of the Stealth PubKey.


namespace _2._4DarkWallet
{
    class Program
    {

        static void Main()
        {
            BookExamples();
            //EasyImplementation();
        }

        //Same functionality with BookExamples by using 2 methods.
        static void EasyImplementation()
        {
            BitcoinStealthAddress address = ReceiverCreateStealthAddress();
            Transaction transaction = SenderCreateTransaction(address);

        }

        //Create stealthAddress.
        static BitcoinStealthAddress ReceiverCreateStealthAddress()
        {
            RandomUtils.Random = new UnsecureRandom();
            var scanKey = new Key();
            var spendKey = new Key();
            BitcoinStealthAddress stealthAddress
                = new BitcoinStealthAddress
                    (
                    scanKey: scanKey.PubKey,
                    pubKeys: new[] { spendKey.PubKey },
                    signatureCount: 1,
                    bitfield: null,
                    network: Network.Main);
            return stealthAddress;
        }

        static Transaction SenderCreateTransaction(BitcoinStealthAddress address)
        {
            Transaction transaction = new Transaction();

            address.SendTo(transaction, new Money(3, MoneyUnit.BTC));

            return transaction;
        }

        static void BookExamples()
        {
            RandomUtils.Random = new UnsecureRandom();

            //Create stealthAddree by several data.
            var scanKey = new Key();
            var spendKey = new Key();
            BitcoinStealthAddress stealthAddress
                = new BitcoinStealthAddress
                    (
                    scanKey: scanKey.PubKey,
                    pubKeys: new[] { spendKey.PubKey },
                    signatureCount: 1,
                    bitfield: null,
                    network: Network.Main);

            //Create epthemKey.
            var ephemKey = new Key();
            //Create a new transaction.
            Transaction transaction = new Transaction();
            //Populate the transaction.
            stealthAddress.SendTo(transaction, Money.Coins(1.0m), ephemKey);
            Console.WriteLine(transaction);

            //The create of the Ephem Key is an implementation detail and I can omit it as NBitcoin will generate one automatically.
            Transaction transaction2 = new Transaction();
            //Notice that there is no ephemKey.
            stealthAddress.SendTo(transaction2, Money.Coins(1.0m));
            Console.WriteLine(transaction2);
        }
    }
}
