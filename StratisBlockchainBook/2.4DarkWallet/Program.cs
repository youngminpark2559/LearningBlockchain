using System;
using NBitcoin;
using NBitcoin.Stealth;

//Dark wallet is nothing to do with dark.Dark wallet is a practical solution that fixes 2 problems which I had, 1.Delegating key/address generation to an untrusted peer, 2.Prevent outdated backups.
//But Dark wallet has a bonus killer feature.
//Let's suppose that I have to share only one address with the world, let's call this address StealthAddress, without leaking any privacy.
//And let's remind that if I share one BitcoinAddress with everybody, then all can see my balance by consulting the blockchain. That's not the case with a StealthAddress.
//In Dark Wallet terminology, there are feature related to these issues.
//The payer know the StealthAddress of the receiver.
//The receiver know the spend key and a secret which will allow him to spend the coins which he receives from such a transaction.
//Scanner know the Scan Key and a secret which allows him to detect the transactions which belong to the Receiver.
//The rest is operational details.
//Underneath, this StealthAddress is composed of one or several Spen PubKey(for multi sig), and one Scan PubKey.




//The payer will take your StealthAddress, and generate a temporary key called Ephem Key, and generate a Stealth Pub Key, from which the Bitcoin address to which the payment will be made is generated.

//Then, they will package the Ephem PubKey in a Stealth Metadata object which is embedded in the OP_RETURN of the transaction(as I did for the first challenge).
//They will also add the output to the generated bitcoin address which is the address of the Stealth PubKey.

//The create of the Ephem Key is an implementation detail and I can omit it as NBitcoin will generate one automatically.
namespace _2._4DarkWallet
{
    class Program
    {
        static void Main()
        {
            BookExamples();
            EasyImplementation();
        }

        static void EasyImplementation()
        {
            BitcoinStealthAddress address = ReceiverCreateStealthAddress();
            Transaction transaction = SenderCreateTransaction(address);

        }

        static BitcoinStealthAddress ReceiverCreateStealthAddress()
        {
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

            var ephemKey = new Key();
            Transaction transaction = new Transaction();
            stealthAddress.SendTo(transaction, Money.Coins(1.0m), ephemKey);
            Console.WriteLine(transaction);

            Console.ReadLine();
        }
    }
}
