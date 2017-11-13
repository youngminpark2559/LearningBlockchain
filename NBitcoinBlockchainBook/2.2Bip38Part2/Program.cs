using System;
using NBitcoin;

//c Add project. I can use features of BIP38 in 2 ways. First, I can use it to encrypt a key. Second, I can use it to delegate "Key and Address creation" to an untrusted peer. This idea can be executed by generating a PassphraseCode to the key generator. With this PassphraseCode, they will be able to generate encrypted keys on the behalf of me, without knowing my password, or any private key. This PassphraseCode can be given to my key generator in WIF format. For reference, in NBitcoin, all types prefixed by "Bitcoin" are Base58(WIF) data. So, if I want to delegate key creation, first I need to create the PassphraseCode.

namespace _2._2Bip38
{
    class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();

            //First, I create BitcoinPassphraseCode
            //PassphraseCode = Password + Network.
            var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);

            //And then, I give this PassphraseCode to a third party key generator. And then, the third party key generator will generate new encrypted keys for me.
            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();

            //encryptedKeyResult has a lot of informations.
            //First information is the generated bitcoin address.
            var generatedAddress = encryptedKeyResult.GeneratedAddress;
            //Second, this is the encryptedKey itself.
            var encryptedKey = encryptedKeyResult.EncryptedKey;
            //This is the confirmationCode, so that the third party can prove that the generated key and address correspond to my password.
            var confirmationCode = encryptedKeyResult.ConfirmationCode;

            Console.WriteLine(generatedAddress); // 14KZsAVLwafhttaykXxCZt95HqadPXuz73
            Console.WriteLine(encryptedKey); // 6PnWtBokjVKMjuSQit1h1Ph6rLMSFz2n4u3bjPJH1JMcp1WHqVSfr5ebNS
            Console.WriteLine(confirmationCode); // cfrm38VUcrdt2zf1dCgf4e8gPNJJxnhJSdxYg6STRAEs7QuAuLJmT5W7uNqj88hzh9bBnU9GFkN

            //As the owner of this, once I receive this information, I need to check whether the key generator did not cheat, by using ConfirmationCode.Check, then I get my private key with my password.
            Console.WriteLine(confirmationCode.Check("my secret", generatedAddress)); // True
            //Get bitcoinPrivateKey.
            var bitcoinPrivateKey = encryptedKey.GetSecret("my secret");
            Console.WriteLine(bitcoinPrivateKey.GetAddress() == generatedAddress); // True
            Console.WriteLine(bitcoinPrivateKey); // KzzHhrkr39a7upeqHzYNNeJuaf1SVDBpxdFDuMvFKbFhcBytDF1R

            //I've seen how the third party can generate encrypted keys on the behalf of myself, without knowing my password and private key.
            //However, one problem is emerged. All backups of my wallet that I own will become outdated when I generate a new key.
            //BIP 32, or Hierarchical Deterministic Wallets(HD wallets), proposes another solution, which is more widely supported.
        }
    }
}
