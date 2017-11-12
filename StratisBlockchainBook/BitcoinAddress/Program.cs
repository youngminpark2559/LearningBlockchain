using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinAddress
{
    public class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();

            // generate a random private key
            Key privateKey = new Key();
            Console.WriteLine(privateKey);

            //On the private key, I use a one-way cryptographic function, to generate a public key.
            PubKey publicKey = privateKey.PubKey;
            Console.WriteLine(publicKey);

            //I can generate public bitcoin address by using publickey and network type. Note that public address for mainnet starts with "1", public address for testnet starts with "m".
            Console.WriteLine(publicKey.GetAddress(Network.Main)); // 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
            Console.WriteLine(publicKey.GetAddress(Network.TestNet)); // n3zWAo2eBnxLr3ueohXnuAa8mTVBhxmPhq


            //To be precise, a bitcoin address is made of a version byte(which is different from the both networks) and your public key's hash bytes. Both of these bytes are concatenated and then encoded into a Base58Check.
            //Underlying, publickey is hashed twice, first it's hashed by SHA256, second on its result, it's hashed by RIPEMD160 with using Big Endian notation.
            //The process can be expressed like this:
            //RIPEMD160(SHA256(publickey))
            var publicKeyHash = publicKey.Hash;
            // f6889b21b5540353a29ed18c45ea0031280c42cf
            Console.WriteLine(publicKeyHash); 
            
            //Underlying this statement, publickeyHash(bytes), a version byte are concatenated and then encoded into a Base58Check. 
            var mainNetAddress = publicKeyHash.GetAddress(Network.Main);
            var testNetAddress = publicKeyHash.GetAddress(Network.TestNet);

            Console.WriteLine(mainNetAddress); // 1PUYsjwfNmX64wS368ZR5FMouTtUmvtmTY
            Console.WriteLine(testNetAddress); // n3zWAo2eBnxLr3ueohXnuAa8mTVBhxmPhq
        }
    }
}
