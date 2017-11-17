using System;
using NBitcoin;
using NBitcoin.Crypto;
// ReSharper disable All

namespace _2._1KeyEncryption
{
    class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();

            //============================================================================================
            //Sectioin. Key generation and encryption



            //============================================================================================
            //Section1. Is it random enough?

            //When you call new Key(), under the hood, you are using a PRNG (Pseudo - Random - Number - Generator) to generate your private key.On windows, it uses the RNGCryptoServiceProvider, a.NET wrapper around the Windows Crypto API.
            //    On Android, I use the SecureRandom class, and in fact, you can use your own implementation with RandomUtils.Random.
            //    On iOS, I have not implemented it and you will need to create your own IRandom implementation.
            //    For a computer, being random is hard.But the biggest issue is that it is impossible to know if a series of numbers is really random.
            //If malware modifies your PRNG so that it can predict the numbers you will generate, you won’t see it until it is too late.
            //It means that a cross platform and naïve implementation of PRNG (like using the computer’s clock combined with CPU speed) is dangerous.But you won’t see it until it is too late.
            //For performance reasons, most PRNG works the same way: a random number which is called a Seed is chosen, then a predictable formula generates the next number each time you ask for it.
            //The amount of randomness of the seed is defined by a measure we call Entropy, but the amount of Entropy also depends on the observer.
            //Let’s say you generate a seed from your clock time.
            //And let’s imagine that your clock has 1ms of resolution. (Reality is more ~15ms.)
            //If your attacker knows that you generated the key last week, then your seed has
            //1000 * 60 * 60 * 24 * 7 = 604800000 possibilities.
            //For such attacker, the entropy is log2(604800000) = 29.17 bits.
            //And enumerating such all possibility with corresponding entropy on my home computer took less than 2 seconds. We call such enumeration “brute forcing”.
            //However, let’s say, you use the clock time + the process ID for generating the seed.
            //Let’s imagine that there are 1024 different process IDs.
            //So now, the attacker needs to enumerate 604800000 * 1024 possibilities, which take around 2000 seconds.
            //Now, let’s add the time on it. When I turned on my computer, assuming the attacker knows I turned it on today, it adds 86400000 possibilities.
            //Now the attacker needs to enumerate 604800000 * 1024 * 86400000 = 5,35088E+19 possibilities.
            //However, keep in mind that if the attacker has infiltrated my computer, he can get this last piece of info, and bring down the number of possibilities, reducing entropy.
            //Entropy is measured by log2(possibilities) and so log2(5,35088E+19) = 65 bits.
            //Is it enough? Probably, only when you assuming your attacker does not know more information about the realm of possibilities used to generate the seed.
            //But since the hash of a public key is 20 bytes(160 bits), it is smaller than the total universe of the addresses.You might do better.
            //Note: Adding entropy is linearly harder. On the other hand, cracking entropy is exponentially harder.
            //An interesting way of generating entropy quickly is by incorporating human intervention, such as moving the mouse.+


            //If you don’t completely trust the platform PRNG (which is not so paranoic), you can add entropy to the PRNG output that NBitcoin is using.

            //What NBitcoin does when you call AddEntropy(data) is
            //additionalEntropy = SHA(SHA(data)^additionalEntropy)
            RandomUtils.AddEntropy("hello");
            RandomUtils.AddEntropy(new byte[] { 1, 2, 3 });
            //When you generate a new number it's like
            //result = SHA(PRNG()^additionalEntropy)
            var nsaProofKey = new Key();
            Console.WriteLine(nsaProofKey.GetWif(Network.Main));




            //=======================================================================================
            //Section2. Key Derivation Function

            //However, what is most important is not the number of possibilities.It is the time that an attacker would need to successfully break your key. That’s where KDF enters the game.
            //KDF, or Key Derivation Function, is a way to have a stronger key, even if your entropy is low.
            //Imagine that you want to generate a seed, and the attacker knows that there are 10,000,000 possibilities.
            //Such a seed would be normally cracked pretty easily.
            //But what if you could make the enumeration slower?
            //A KDF is a hash function that wastes computing resources on purpose.
            //Here is an example:
            var derived = SCrypt.BitcoinComputeDerivedKey("hello", new byte[] { 1, 2, 3 });
            RandomUtils.AddEntropy(derived);



            //Even if your attacker knows that your source of entropy is 5 letters, he will need to run Scrypt to check each possibility, which takes 5 seconds on my computer.
            //The bottom line is: There is nothing paranoid in distrusting a PRNG, and you can mitigate an attack by both adding entropy and also using a KDF.
            //Keep in mind that an attacker can decrease entropy by gathering information about you or your system.
            //If you use the timestamp as entropy source, then an attacker can decrease the entropy by knowing the fact that you generated the key last week, and that you only use your computer between 9am and 6pm.
            //In the previous part I talked briefly about a special KDF called Scrypt. As I said, the goal of a KDF is to make "brute forcing" costly.
            //So it should be no surprise for you that a standard already exists for encrypting your private key with a password using a KDF.This is BIP38.

            var privateKey = new Key();
            var bitcoinPrivateKey = privateKey.GetWif(Network.Main);
            Console.WriteLine(bitcoinPrivateKey); 
            //Output:
            //L1tZPQt7HHj5V49YtYAMSbAmwN9zRjajgXQt9gGtXhNZbcwbZk2r
            BitcoinEncryptedSecret encryptedBitcoinPrivateKey = bitcoinPrivateKey.Encrypt("password");
            Console.WriteLine(encryptedBitcoinPrivateKey);
            //Output:
            //6PYKYQQgx947Be41aHGypBhK6TA5Xhi9TdPBkatV3fHbbKrdDoBoXFCyLK
            var decryptedBitcoinPrivateKey = encryptedBitcoinPrivateKey.GetSecret("password");
            Console.WriteLine(decryptedBitcoinPrivateKey);
            //Output:
            //L1tZPQt7HHj5V49YtYAMSbAmwN9zRjajgXQt9gGtXhNZbcwbZk2r



            //Such encryption is used in two different cases:
            //You do not trust your storage provider(they can get hacked)
            //You are storing the key on the behalf of somebody else (and you do not want to know their key)
            //If you own your storage, then encrypting at the database level might be enough.
            //Be careful if your server takes care of decrypting the key. An attacker might attempt a DDoS attack to your server by forcing it to decrypt lots of keys.
            //Delegate decryption to the ultimate end user when you can.





            //=======================================================================================
            //Section3. Like the good ol’ days

            //First, why generate several keys?
            //The main reason is privacy.Since you can see the balance of all addresses, it is better to use a new address for each transaction.
            //However, in practice, you can also generate keys for each contact which makes this a simple way to identify your payer without leaking too much privacy.
            //You can generate a key, like you did at the beginning:
            var privateKey1 = new Key();


            //However, you have two problems with that:
            //1.All backups of your wallet that you have will become outdated when you generate a new key.
            //2.You cannot delegate the address creation process to an untrusted peer.
            //If you are developing a web wallet and generate keys on behalf of your users, and one user get hacked, they will immediately start suspecting you.
        }
    }
}
