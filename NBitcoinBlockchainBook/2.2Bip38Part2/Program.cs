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

            //====================================================================================
            //Section. BIP38(Part 2)

            //We already looked at using BIP38 to encrypt a key. However this BIP is in reality two ideas in one document.
            //The second part of the BIP shows how you can delegate Key and Address creation to an untrusted peer. It will fix one of our concerns.
            //The idea is to generate a PassphraseCode to the key generator. With this PassphraseCode, they will be able to generate encrypted keys on your behalf, without knowing your password, nor any private key.
            //This PassphraseCode can be given to your key generator in WIF format.
            
            //Tip: In NBitcoin, all types prefixed by “Bitcoin” are Base58 (WIF) data.
            
            //So, as a user that wants to delegate key creation, first you will create the PassphraseCode.
            //Illustration:
            //Password + Network => PassphraseCode

            var passphraseCode = new BitcoinPassphraseCode("my secret", Network.Main, null);

            //You then give this PassphraseCode to a third party key generator. And then, the third party key generator will generate new encrypted keys for me.
            //Illustration:
            //PassphraseCode => EncryptedKey1, EncryptedKey2 
            EncryptedKeyResult encryptedKeyResult = passphraseCode.GenerateEncryptedSecret();

            //This EncryptedKeyResult class has lots of information:
            //Illustration:






            //First is the generated Bitcoin address.
            var generatedAddress = encryptedKeyResult.GeneratedAddress;
            //Output:
            //14KZsAVLwafhttaykXxCZt95HqadPXuz73

            //Second is the encryptedKey itself as we have seen in the previous, Key Encryption lesson.
            var encryptedKey = encryptedKeyResult.EncryptedKey;

            //And last but not least, the ConfirmationCode, so that the third party can prove that the generated key and address correspond to my password.
            var confirmationCode = encryptedKeyResult.ConfirmationCode;
            //Output:
            //cfrm38VUcrdt2zf1dCgf4e8gPNJJxnhJSdxYg6STRAEs7QuAuLJmT5W7uNqj88hzh9bBnU9GFkN

            Console.WriteLine(generatedAddress); 
            //Output:
            //14KZsAVLwafhttaykXxCZt95HqadPXuz73
            Console.WriteLine(encryptedKey); 
            //Output:
            //6PnWtBokjVKMjuSQit1h1Ph6rLMSFz2n4u3bjPJH1JMcp1WHqVSfr5ebNS
            Console.WriteLine(confirmationCode);
            //Output:
            //cfrm38VUcrdt2zf1dCgf4e8gPNJJxnhJSdxYg6STRAEs7QuAuLJmT5W7uNqj88hzh9bBnU9GFkN




            //As the owner, once you receive this information, you need to check that the key generator did not cheat by using ConfirmationCode.Check, then get your private key with your password:
            Console.WriteLine(confirmationCode.Check("my secret", generatedAddress));
            //Output:
            //True

            //Get bitcoinPrivateKey by a password.
            var bitcoinPrivateKey = encryptedKey.GetSecret("my secret");
            Console.WriteLine(bitcoinPrivateKey.GetAddress() == generatedAddress);
            //Output:
            //True
            Console.WriteLine(bitcoinPrivateKey);
            //Output:
            //KzzHhrkr39a7upeqHzYNNeJuaf1SVDBpxdFDuMvFKbFhcBytDF1R

            //So, we have just seen how the third party can generate encrypted keys on your behalf, without them knowing your password and private key.
            //In other words, you've delegated a Key and Address creation to an untrusted peer, the third party.
            //Illustration:



            //However, one problem remains:
            //All backups of your wallet that you have will become outdated when you generate a new key.
            //BIP 32, or Hierarchical Deterministic Wallets (HD wallets), proposes another solution which is more widely supported.
        }
    }
}
