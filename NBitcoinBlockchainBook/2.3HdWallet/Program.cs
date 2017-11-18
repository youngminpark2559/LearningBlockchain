using System;
using NBitcoin;

//c Add project. I got problem to be resolved in previous example. I should acheive 2 requirements. 1.Prevent outdated backups, 2.Delegating key/address generation to an untrusted peer. A "Deterministic" wallet would fix the backup problem. With such a wallet, I would have to save only the seed. From this seed, I can generate the same series of private keys over and over. From the master key, I can generate the new keys.

namespace HdWallet
{
    internal class Program
    {
        private static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();


            //==========================================================================================
            //Chapter. HD Wallet(BIP 32)

            //Let’s keep in mind the problems that we want to resolve:
            //1.Prevent outdated backups
            //2.Delegating key / address generation to an untrusted peer
            //A “Deterministic” wallet would fix our backup problem. With such a wallet, you would have to save only the seed.From this seed, you can generate the same series of private keys over and over.
            //This is what the “Deterministic” stands for.
            //As you can see, from the master key, I can generate new keys:

            //Create a masterKey.
            ExtKey masterKey = new ExtKey();
            Console.WriteLine("Master key : " + masterKey.ToString(Network.Main));
            //Output:
            //xprv9s21ZrQH143K46gx2C5V4o9iEn52h9y2Y7LykXSTVPFLM28kyMuj8BkicTd3uAiqrVPxb3BZ4fVkzKDwzxVpkEhsqS5HD6vrGDf5D613Lwt

            //Create 5 derived keys based on the masterKey.
            for (int i = 0; i < 5; i++)
            {
                ExtKey key = masterKey.Derive((uint)i);
                Console.WriteLine("Key " + i + " : " + key.ToString(Network.Main));
            }
            //Output:
            //Key 0 : xprv9uvuViKQnT4vo8rSWEtXYXxDLMvt4SrSToHVs3ZpzKXpHd5deZNDKj57XnrQ8rRZSANm3oZBjXN245oy161otLGddxch46UrtTH3tB5fMKQ
            //Key 1 : xprv9uvuViKQnT4vqG7czoZ1VYGPmK9yhXBLGYKgZpiB271L6ekwbzyPo9fHE3F4cAay6qbwczt1K35cEF3HrykPGR9agzmHNhBPDSZtyoxhMbh
            //Key 2 : xprv9uvuViKQnT4vs7GpRKcouP8TP8ERksBG5GdNmocVDp6R8nHGTYH4qwG8PY75hHGKsTKQdQmRAiQnZim6UuYguiaUx1dpqGXafM1Lr1WEkzy
            //Key 3 : xprv9uvuViKQnT4vwDWmEvSEae5nLGbKLMmeMLLCc5MDD9iVSDhDqNpzSErHBYm1TRZGfDM2YbHZyCtmas1d2JRSWMYLKbJMGhyB9ynSbyLTh14
            //Key 4 : xprv9uvuViKQnT4vxgXFvxk6FhKYgzU8NgW3xMfqciveTWKpJDkdCmQiJjAdGUij7QckJ1ZBqKuY1RSyw9JeGM7P6cTMPZqVZBu9SCAfHEpp8SS



            //You only need to save the masterKey, since you can generate the same suite of private keys over and over.
            //As you can see, these keys are ExtKey and not Key as you are used to. However, this should not stop you since you have the real private key inside of these keys:


            //You can go back from a Key to an ExtKey by supplying the Key and the ChainCode to the ExtKey constructor. This works as follows:

            //Create extKey.
            ExtKey extKey = new ExtKey();
            //Get ChainCode from extKey.
            byte[] chainCode = extKey.ChainCode;
            //Get a PrivateKey from extKey.
            Key privateKeyFromExtKey = extKey.PrivateKey;
            //Supply a private key and ChainCode to the ExtKey constructor to go back from a Key to an ExtKey.
            ExtKey newExtKey = new ExtKey(privateKeyFromExtKey, chainCode);


            //The Base58 type equivalent of ExtKey is called BitcoinExtKey.
            //But how can we solve our second problem: Delegating address creation to a peer that can potentially be hacked like a payment server?
            //The trick is that you can “neuter” your master key, then you have a public (without private key) version of the master key.From this neutered version, a third party can generate public keys without knowing the private key.

            //Neuter the master key, then you get a master public key.
            ExtPubKey masterPubKey = masterKey.Neuter();

            //Genarate 5 derived public keys from the master public key.
            for (int i = 0; i < 5; i++)
            {
                ExtPubKey pubkey = masterPubKey.Derive((uint)i);
                Console.WriteLine("PubKey " + i + " : " + pubkey.ToString(Network.Main));
            }

            //So imagine that your payment server generates pubkey1, and then you can get the corresponding private key with your private master key.
            masterKey = new ExtKey();
            masterPubKey = masterKey.Neuter();

            //The payment server generates pubkey1.
            ExtPubKey pubkey1 = masterPubKey.Derive((uint)1);

            //You get the private key of pubkey1
            ExtKey key1 = masterKey.Derive((uint)1);

            //Check if it is legit
            Console.WriteLine("Generated address : " + pubkey1.PubKey.GetAddress(Network.Main));
            Console.WriteLine("Expected address : " + key1.PrivateKey.PubKey.GetAddress(Network.Main));
            //Generated address: 1Jy8nALZNqpf4rFN9TWG2qXapZUBvquFfX
            //Expected address:	 1Jy8nALZNqpf4rFN9TWG2qXapZUBvquFfX

            //ExtPubKey is similar to ExtKey except that it holds a public key and not a private key.



            //Now we have seen how Deterministic keys solve our problems, let’s speak about what the “hierarchical” is for.

            //In the previous exercise, we have seen that by combining master key + index we could generate another key.We call this process Derivation, the master key is the parent key, and any generated keys are called child keys.

            //However, you can also derivate children from the child key.This is what the “hierarchical” stands for.

            //This is why conceptually more generally you can say: Parent Key + KeyPath => Child Key






            //Now that we have seen how Deterministic keys solve our problems, let’s speak about what the “hierarchical” is for.
            //In the previous exercise, we have seen that we could generate another derived keys based on a master key by invoking Derive() method on the master key passing integer numbers into an argument.
            //We call this process a Derivation. And in this scheme, a master key is a parent key, and any generated keys based on the master key are called child keys.
            //However, I can also derivate children from the "child key". This is what the “Hierarchical” stands for. 
            //This is why conceptually more generally you can say: Parent Key + KeyPath => Child Key

            //Just suppose the scenario that there is a parent key, "Parent".
            //And there can be child keys derived from "Parent". => Child(1),Child(2),Child(3),Child(4). 
            //And there can be child keys derived from Child(1). => Child(1, 1), Child(1, 2).

            //In this diagram, you can derivate Child(1,1) from a parent in two different way:
            //First generate a parent key.
            ExtKey parent = new ExtKey();

            ExtKey child11ByFirstWay = parent.Derive(1).Derive(1);
            Console.WriteLine(child11ByFirstWay);

            //Or above code can be expressed in this way, resulting in an identical output.
            ExtKey child11BySecondWay = parent.Derive(new KeyPath("1/1"));
            Console.WriteLine(child11BySecondWay);

            //Remember that Ancestor ExtKey + KeyPath = Child ExtKey.

            //This process works indenticaly for ExtPubKey.

            //Why do I need hierarchical keys? It's because it might be a nice way to classify the type of my keys for multiple accounts. This point is more neat than on BIP44. It also permits segmenting account rights across an organization. 

            //Let's imagine a scenario that I'm a CEO of a company. I want to control over all wallets. In this point, I don't want the Accounting department to spend the money from the Marketing department. For implementing this constraint, the first idea would be to generate one hierarchy for each department.

            //CEO Key(the master key)-> derived child Keys from a master key(the CEO key) : Marketing(0), Accounting(0).
            //Marketing(0)->Child Keys:Marketing(0, 1), Marketing(0, 2).
            //Accounting(0)->Child Keys:Accounting(0, 2), Accounting(0, 2).

            //However, in such case, one problem comes that Accounting and Marketing would be able to recover the CEO's private key. In above code, I defined such child keys as non-hardened.

            //Parent ExtPubKey + Child ExtKey(non hardened) => Parent ExtKey.


            ExtKey ceoKey = new ExtKey();
            Console.WriteLine("CEO: " + ceoKey.ToString(Network.Main));
            //Note the hardened is false.
            ExtKey accountingKey = ceoKey.Derive(0, hardened: false);

            ExtPubKey ceoPubkey = ceoKey.Neuter();

            //Recover the CEO key by the accounting private key and the CEO public key.
            ExtKey ceoKeyRecovered = accountingKey.GetParentExtKey(ceoPubkey);
            Console.WriteLine("CEO recovered: " + ceoKeyRecovered.ToString(Network.Main));
            //CEO: xprv9s21ZrQH143K2XcJU89thgkBehaMqvcj4A6JFxwPs6ZzGYHYT8dTchd87TC4NHSwvDuexuFVFpYaAt3gztYtZyXmy2hCVyVyxumdxfDBpoC
            //CEO recovered: xprv9s21ZrQH143K2XcJU89thgkBehaMqvcj4A6JFxwPs6ZzGYHYT8dTchd87TC4NHSwvDuexuFVFpYaAt3gztYtZyXmy2hCVyVyxumdxfDBpoC

            //In other words, a non-hardened key can “climb” the hierarchy. Non-hardened keys should only be used for categorizing accounts that belong to a point of single control.

            //So in our case, the CEO should create child keys as hardened ones, so the accounting department will not be able to climb the hierarchy.





            //Identical process to above code except for hardened is true.
            Console.WriteLine("CEO: " + ceoKey.ToString(Network.Main));
            ExtKey accountingKeyHardened = ceoKey.Derive(0, hardened: true);
            Console.WriteLine(accountingKeyHardened);
            //Generate derived child accountKeys from ceoKey
            //However, since accountKeys are hardened, they can't climb hierarchy towards ceoKey.
            ExtPubKey ceoPubkeyToTestForHardened = ceoKey.Neuter();
            Console.WriteLine(ceoPubkeyToTestForHardened);
            ////At this point, it'll be crashed by this climbing attempt.
            //ExtKey ceoKeyRecovered = accountingKeyHardened.GetParentExtKey(ceoPubkeyToTestForHardened); 


            
            //You can also create hardened keys via the ExtKey.Derivate(KeyPath), by using an apostrophe(') after a child’s index such as "1/2/3'"
            var nonHardenedChildKey = new KeyPath("1/2/3");
            var hardenedChildKey = new KeyPath("1/2/3'");
            Console.WriteLine(nonHardenedChildKey);
            Console.WriteLine(hardenedChildKey);


            //So let’s imagine that the Accounting Department generates 1 parent key for each customer, and a child for each of the customer’s payments.

            //As the CEO, you want to spend the money on one of these addresses.Here is how you would proceed.
            ceoKey = new ExtKey();
            //Child key is generated as hardened, so it can't climb hierarchy upwards to a parent key.
            string accounting = "1'";
            int customerId = 5;
            int paymentId = 50;
            KeyPath path = new KeyPath(accounting + "/" + customerId + "/" + paymentId);
            //Path will be "1'/5/50"
            ExtKey paymentKey = ceoKey.Derive(path);
            Console.WriteLine(paymentKey);


            //===========================================================================================
            //Chapter. Mnemonic Code for HD Keys (BIP39)


            //I've seen how to generate HD keys. However, what if I want an easy way to transmit such a key by telephone or hand writing?
            //Cold wallets such as Trezor generate the HD keys from a sentence that can be easily memorized or written down. They call such a sentence "the seed" or "mnemonic”. And it can eventually be protected by a password or a PIN.
            //The thing that I use to generate my "easy to memorize and write" sentence is called a Wordlist.
            //Wordlist+mnemonic+password=>HD Root key.
            Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
            ExtKey hdRoot1 = mnemo.DeriveExtKey("my password");
            Console.WriteLine(mnemo);
            Console.WriteLine(hdRoot1);

            //Now, if I have the mnemonic and the password, I can recover the hdRoot key.
            mnemo = new Mnemonic("minute put grant neglect anxiety case globe win famous correct turn link", Wordlist.English);
            ExtKey hdRoot2 = mnemo.DeriveExtKey("my password");
            Console.WriteLine(hdRoot2);
        }
    }
}