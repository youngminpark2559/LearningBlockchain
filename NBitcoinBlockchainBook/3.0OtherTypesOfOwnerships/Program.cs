using System;
using System.Linq;
using System.Text;
using NBitcoin;
using NBitcoin.Crypto;

namespace _3._0OtherTypesOfOwnerships
{
    class Program
    {
        static void Main()
        {
            RandomUtils.Random = new UnsecureRandom();

            //c Bitcoin Address is the hash of a publicKey encoded in Base58.
            var publicKeyHash = new Key().PubKey.Hash;
            var bitcoinAddress = publicKeyHash.GetAddress(Network.Main);
            Console.WriteLine(publicKeyHash); // 41e0d7ab8af1ba5452b824116a31357dc931cf28
            Console.WriteLine(bitcoinAddress); // 171LGoEKyVzgQstGwnTHVh3TFTgo5PsqiY


            //We also learned that as far as the blockchain is concerned, there is no such thing as a bitcoin address. 
            //The blockchain identifies a receiver with a ScriptPubKey, and that a ScriptPubKey could be generated from the address.

            //Create scriptPubKey from bitcoinAddress.
            var scriptPubKey = bitcoinAddress.ScriptPubKey;
            Console.WriteLine(scriptPubKey);
            //Output:
            // OP_DUP OP_HASH160 41e0d7ab8af1ba5452b824116a31357dc931cf28 OP_EQUALVERIFY OP_CHECKSIG
            //And vice versa. BitcoinAddress could be generated from a ScriptPubKey and Network.
            var sameBitcoinAddress = scriptPubKey.GetDestinationAddress(Network.Main);
            Console.WriteLine($"sameBitcoinAddress {sameBitcoinAddress}");



            //==========================================================================================
            //About P2PK

            //However, not all ScriptPubKey represents a Bitcoin Address.
            //For example, there is the first transaction in the first ever blockchain block which is called genesis block. 

            //Get genesisBlock from mainnet.
            Block genesisBlock = Network.Main.GetGenesis();
            //Get the firstTransaction from genesisBlock
            Transaction firstTransactionEver = genesisBlock.Transactions.First();
            Console.WriteLine(firstTransactionEver);
            //{
            //…
            //  "out": [
            //    {
            //      "value": "50.00000000",
            //      "scriptPubKey": "04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG"
            //    }
            //  ]
            //}
            //Note that the form of the scriptPubKey is different from ordinary one.
            //A bitcoin address is represented by: 
            //OP_DUP OP_HASH160 <hash> OP_EQUALVERIFY OP_CHECKSIG
            //But here we have:
            //<pubkey> OP_CHECKSIG
            //In fact, at the beginning, public keys were already used directly in the ScriptPubKey.


            var firstOutputEver = firstTransactionEver.Outputs.First();
            var firstScriptPubKeyEver = firstOutputEver.ScriptPubKey;
            Console.WriteLine(firstScriptPubKeyEver);
            //04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f OP_CHECKSIG


            var firstBitcoinAddressEver = firstScriptPubKeyEver.GetDestinationAddress(Network.Main);
            Console.WriteLine(firstBitcoinAddressEver == null); // True

            var firstPubKeyEver = firstScriptPubKeyEver.GetDestinationPublicKeys().First();
            Console.WriteLine(firstPubKeyEver); // 04678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5f



            //Note that what P2PK(Pay To Public Key) and P2PKH(Pay To Public Key Hash) are.
            //P2PK : PrivateKey->Public Key<->ScriptPubKey.
            //P2PKH : PrivateKey->Public Key->Public Key hash<->ScriptPubKey.

            var key = new Key();
            Console.WriteLine("Pay To Public Key : " + key.PubKey.ScriptPubKey);
            Console.WriteLine("Pay To Public Key Hash : " + key.PubKey.Hash.ScriptPubKey);
            //Pay to public key : 02fb8021bc7dedcc2f89a67e75cee81fedb8e41d6bfa6769362132544dfdf072d4 OP_CHECKSIG
            //Pay to public key hash : OP_DUP OP_HASH160 0ae54d4cec828b722d8727cb70f4a6b0a88207b2 OP_EQUALVERIFY OP_CHECKSIG

            //These 2 types of payment way are referred as P2PK(Pay To Public Key) and P2PKH(Pay To Public Key Hash).
            //Satoshi later decided to use P2PKH instead of P2PK for 2 reasons.
            //1. Elliptic curve cryptography which is used by public key and private key is vulnerable to a modified Shor's algorithm for solving the discrete logarithm problem on elliptic curves.
            //It means that in the future a quantum computer might be able to retrieve a private key from a public key.
            //By publishing the public key only when the coins are spent(at this point, also assume that the addresss are not reused), such an attack is rendered ineffective.
            //2. With the hash being smaller than 20 bytes, it's easier to print and embed into small storage mediums like QR codes.
            //Nowadays, there's no reason to use P2PK directly although it's still used in combination with P2SH. P2SH is talked more in detail in later chapter.
            //If the issue of the early use of P2Pk is not addressed, it will have a serious impact on the Bitcoin price.



            //============================================================================================

            //P2WPKH(Pay to Witness Public Key Hash)
            //In 2015, Pieter Wuille introduced a new feature to bitcoin called Segregated Witness, also known by it's abbreviated name, Segwit. Basically, Segregated Witness moves the proof of ownership from the scriptSig part of the transaction to a new part called the witness of the input.
            //There are several reasons why it is beneficial to use this new scheme, a summary of which are presented below.For more details visit https://bitcoincore.org/en/2016/01/26/segwit-benefits/.
            //1.Third party Malleability Fix: Previously, a third party could change the transaction id of your transaction before it was confirmed.This can not occur under Segwit.
            //2.Linear sig hash scaling: Signing a transaction used to require hashing the whole transaction for every input. This was a potential DDoS vector attack for large transactions.
            //3.Signing of input values: The amount that is spent in an input is also signed, meaning that the signer can’t be tricked about the amount of fees that are actually being paid.
            //4.Capacity increase: It will now be possible to have more than 1MB of transactions in each block(which are created every 10 minutes on average).Segwit increases this capacity by a factor of about 2.1, based upon the average transaction profile from November 2016.
            //5.Fraud proof: Will be developed later, but SPV wallets will be able to validate more consensus rules rather than just simply following the longest chain.

            //Before Sewgit, in the transaction, signature was used in the calculation of the transaction id.
            //At that time, signature was located in TxIn, but after Segwi
            //The signature contains the same information as a P2PKH spend, but signature is located in the witness instead of the scriptSig. The scriptPubKey though, is modified from
            //OP_DUP OP_HASH160 0067c8970e65107ffbb436a49edd8cb8eb6b567f OP_EQUALVERIFY OP_CHECKSIG
            //To
            //0 0067c8970e65107ffbb436a49edd8cb8eb6b567f



            //For nodes which did not upgrade, this looks like two pushes on the stack. This means that any scriptSig can spend them. So even without the signatures, old nodes will consider such transactions valid. New nodes interpret the first push as the witness version and the second push as the witness program.
            //New nodes will therefore also require the signature in order to verify the transaction.
            //In NBitcoin, spending a P2WPKH output is no different from spending a normal P2PKH.
            //To get the ScriptPubKey from a public key simply use PubKey.WitHash instead of PubKey.Hash.
            var key1 = new Key();
            Console.WriteLine(key1.PubKey.WitHash.ScriptPubKey);
            //Which will output something like
            //0 0067c8970e65107ffbb436a49edd8cb8eb6b567f


            //Signing the spending of such coins will be explained later in the “Using the TransactionBuilder" section, and does not differ in any way from the code used to sign a P2PKH output.
            //The witness data is similar to the scriptSig of P2PKH, and the scriptSig data is empty:

            // "in": [
            // {
            // "prev_out": 
            // {
            // "hash": "725497eaef527567a0a18b310bbdd8300abe86f82153a39d2f87fef713dc8177",
            // "n": 0
            // },
            // "scriptSig": "",
            // "witness": "3044022079d443be2bd39327f92adf47a34e4b6ad7c82af182c71fe76ccd39743ced58cf0220149de3e8f11e47a989483f371d3799a710a7e862dd33c9bd842c417002a1c32901 0363f24cd2cb27bb35eb2292789ce4244d55ce580218fd81688197d4ec3b005a67"
            // }

            //Once again, the semantics of P2WPKH is the same as the semantics of P2PKH, except that the signature is not placed at the same location as before.




            //=========================================================================================
            //MUSTISIG 

            //It is possible to have shared ownership and control over coins.
            //In order to demonstrate this, we will create a ScriptPubKey that represents an m-of-n multi sig. 
            //This means that in order to spend the coins, m number of private keys will need to sign the spending transaction out of the n number of different public keys provided.
            //Let’s create a multi sig with Bob, Alice and Satoshi, where two of the three of them need to sign a transaction in order to spend a coin.


            Key bob = new Key();
            Key alice = new Key();
            Key satoshi = new Key();

            Console.WriteLine($"bob.PubKey {bob.PubKey}");
            Console.WriteLine($"alice.PubKey {alice.PubKey}");
            Console.WriteLine($"satoshi.PubKey {satoshi.PubKey}");
            //0282213c7172e9dff8a852b436a957c1f55aa1a947f2571585870bfb12c0c15d61 
            //036e9f73ca6929dec6926d8e319506cc4370914cd13d300e83fd9c3dfca3970efb 
            //0324b9185ec3db2f209b620657ce0e9a792472d89911e0ac3fc1e5b5fc2ca7683d


            scriptPubKey = PayToMultiSigTemplate
                .Instance
                .GenerateScriptPubKey(2, new PubKey[]
            {
                bob.PubKey,
                alice.PubKey,
                satoshi.PubKey
            });

            Console.WriteLine(scriptPubKey);
            //Output:
            //2 0282213c7172e9dff8a852b436a957c1f55aa1a947f2571585870bfb12c0c15d61 036e9f73ca6929dec6926d8e319506cc4370914cd13d300e83fd9c3dfca3970efb 0324b9185ec3db2f209b620657ce0e9a792472d89911e0ac3fc1e5b5fc2ca7683d 3 OP_CHECKMULTISIG
            //As you can see, the scriptPubkey has the following form: 
            //<sigsRequired> <pubkeys…> <pubKeysCount> OP_CHECKMULTISIG

            //The process for signing, it is a little more complicated than just calling Transaction.Sign, which does not work for multi sig.
            //Later, we will talk more deeply about the subject but for now let’s use the TransactionBuilder for signing the transaction.

            //Imagine that the multi-sig scriptPubKey received a coin in a transaction called received:
            //Transaction: received TxIn:... TxOut: to multi-sig scriptPubKey

            //Create a new transaction and assign it to the variable named recieved.
            var received = new Transaction();
            //Add new TxOut which contains balance of coins, scriptPubKey into TxOut(outputs) of the transaction which is created above, named received.
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), scriptPubKey));


            //Bob and Alice agree to pay Nico 1.0 BTC for his services. 

            //First they get the Coin they received from the transaction.
            //Tip. Txout + OutPoint(TxId+Index of TxOut) => Coin.
            Coin coin = received.Outputs.AsCoins().First();

            //First they get the BitcoinAddress of nico to which they will send Bitcoin.
            BitcoinAddress nico = new Key().PubKey.GetAddress(Network.Main);
            //Then, with the TransactionBuilder, they create an unsigned transaction.
            TransactionBuilder builder = new TransactionBuilder();
            TransactionBuilder builderForAlice = new TransactionBuilder();
            TransactionBuilder builderForBob = new TransactionBuilder();
            Transaction unsigned =
                builder
                    .AddCoins(coin)
                    .Send(nico, Money.Coins(1.0m))
                    .BuildTransaction(sign: false);

            //The transaction is not yet signed. Here is how Alice signs it:
            //Alice privateKey + Coin + Unsigned => Alice signed.
            Transaction aliceSigned =
                builderForAlice
                    .AddCoins(coin)
                    .AddKeys(alice)
                    .SignTransaction(unsigned);
            Console.WriteLine($"======aliceSigned Transaction======\n {aliceSigned}");

            //Here is how Bob signs it on the one which Alice signed:
            //Bob privateKey + Coin + Alice signed => Alice signed.
            Transaction bobSigned =
                builderForBob
                    .AddCoins(coin)
                    .AddKeys(bob)
                    .SignTransaction(unsigned);
            Console.WriteLine($"======bobSigned Transaction======\n {bobSigned}");


            //Now, Bob and Alice can combine their signature into one transaction. This transaction will then be valid in terms of it's signature as Bob and Alice have provided two of the signatures from the three owner signatures that were initially provided. The requirements of the 'two-of-three' multi sig have therefore been met.

            //Coin + CombinedSignature(aliceSigned+bobSigned) => fullySigned.
            Transaction fullySigned =
                builder
                    .AddCoins(coin)
                    .CombineSignatures(aliceSigned, bobSigned);

            Console.WriteLine($"======fullySigned Transaction======\n {fullySigned}");
            //{
            //  ...
            //  "in": [
            //    {
            //      "prev_out": {
            //        "hash": "9df1e011984305b78210229a86b6ade9546dc69c4d25a6bee472ee7d62ea3c16",
            //        "n": 0
            //      },
            //      "scriptSig": "0 3045022100a14d47c762fe7c04b4382f736c5de0b038b8de92649987bc59bca83ea307b1a202203e38dcc9b0b7f0556a5138fd316cd28639243f05f5ca1afc254b883482ddb91f01 3044022044c9f6818078887587cac126c3c2047b6e5425758e67df64e8d682dfbe373a2902204ae7fda6ada9b7a11c4e362a0389b1bf90abc1f3488fe21041a4f7f14f1d856201"
            //    }
            //  ],
            //  "out": [
            //    {
            //      "value": "1.00000000",
            //      "scriptPubKey": "OP_DUP OP_HASH160 d4a0f6c5b4bcbf2f5830eabed3daa7304fb794d6 OP_EQUALVERIFY OP_CHECKSIG"
            //    }
            //  ]
            //}



            //The transaction is now ready to be sent to the network.
            //Even if the Bitcoin network supports multi sig as explained here, one question worth asking is: How can you expect a user who has no clue about bitcoin to pay using the Alice/Bob/Satoshi multi-sig as we have done above?
            //Don’t you think it would be cool if we could represent such a scriptPubKey as easily and concisely as a regular Bitcoin Address?
            //Well, this is possible using something called a Bitcoin Script Address(also called Pay to Script Hash or P2SH for short).
            //Nowadays, native Pay To Multi Sig(as you have seen above) and native P2PK are never used directly.Instead they are wrapped into something called a Pay To Script Hash payment. We will look at this type of payment in the next section.






            //========================================================================================
            //P2SH(Pay To Script Hash)

            //As seen in the previous section, using multi-sig is easily done in code. 
            //However, before P2SH, there was no way to ask someone to pay to a multi-sig scriptPubKey in a way that was as simple as just providing them with a regular BitcoinAddress.
            //Pay To Script Hash(or P2SH as it is often known) is an easy way to represent a scriptPubKey as a simple BitcoinScriptAddress, no matter how complicated it is in terms of its underlying m-of-n signature set up.

            //In the previous part, we generated this multi-sig:
            //Key bob = new Key();
            //Key alice = new Key();
            //Key satoshi = new Key();

            //var scriptPubKey = PayToMultiSigTemplate
            //    .Instance
            //    .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });

            //Console.WriteLine(scriptPubKey);
            //Output:
            //2 0282213c7172e9dff8a852b436a957c1f55aa1a947f2571585870bfb12c0c15d61 036e9f73ca6929dec6926d8e319506cc4370914cd13d300e83fd9c3dfca3970efb 0324b9185ec3db2f209b620657ce0e9a792472d89911e0ac3fc1e5b5fc2ca7683d 3 OP_CHECKMULTISIG

            //Recall that scriptPubKey generated by multiple public key represents in this format:
            //<sigsRequired> <pubkeys…> <pubKeysCount> OP_CHECKMULTISIG
            //3 each publicKeys are appended with whitespaces so that it looks long and complicated, doesn't it?
            //Instead, let’s see how such a scriptPubKey would look shorter by representing such a scriptPubKey in P2SH scriptPubKey format.
            //Key bob = new Key();
            //Key alice = new Key();
            //Key satoshi = new Key();

            //Note that PaymentScript property is appended after GenerateScriptPubKey().
            var paymentScript = PayToMultiSigTemplate
                .Instance
                .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey }).PaymentScript;

            Console.WriteLine(paymentScript);
            //Output:
            //OP_HASH160 57b4162e00341af0ffc5d5fab468d738b3234190 OP_EQUAL

            //Do you see the difference? This P2SH scriptPubKey represents the hash of the multi-sig script: redeemScript.Hash.ScriptPubKey



            //Since it is a hash, you can easily convert it to a base58 string BitcoinScriptAddress.
            //Key bob = new Key();
            //Key alice = new Key();
            //Key satoshi = new Key();

            Script redeemScript =
                PayToMultiSigTemplate
                .Instance
                .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });
            //Console.WriteLine(redeemScript.Hash.ScriptPubKey);
            //The result of above code represents P2SH scriptPubKey generated from redeemScript which is notmal scriptPubKey generated by multiple publicKeys.
            Console.WriteLine(redeemScript.Hash.GetAddress(Network.Main));
            //Output:
            //3E6RvwLNfkH6PyX3bqoVGKzrx2AqSJFhjo

            //Such an address(redeemScript.Hash.GetAddress(Network.Main)) will still be understood by any existing client wallet because it looks like a normal address, even if the wallet does not understand what “multi-sig” is.
            //In P2SH payments, we refer to the hash of the Redeem Script(redeemScript.Hash) as the P2SH scriptPubKey.
            //In other words, Redeem script->Hash of Redeem script->P2SH ScriptPubKey.




            //Since anyone sending a payment to such an address only sees the Hash of the RedeemScript, and do not know the Redeem Script itself, they don’t even have to know that they are sending money to a multi-sig of Alice/Bob/Satoshi.
            //Signing such a transaction is similar to what we have done before. The only difference is that you also have to provide the Redeem Script when you build the Coin for the TransactionBuilder.


            //Imagine that the multi-sig P2SH receives a coin in a transaction called received.

            Script redeemScript1 =
                 PayToMultiSigTemplate
                 .Instance
                 .GenerateScriptPubKey(2, new[] { bob.PubKey, alice.PubKey, satoshi.PubKey });
            received = new Transaction();
            //Pay to the script hash which is represented in P2SH scriptPubKey.
            received.Outputs.Add(new TxOut(Money.Coins(1.0m), redeemScript1.Hash));
            //Warning: The payment is sent to redeemScript.Hash and not to redeemScript1!


            //When any two owners out of the three that control the multi-sig address(Alice/Bob/Satoshi) then want to spend what they have received, instead of creating a Coin, they will need to create a ScriptCoin.

            //Give the redeemScript to the coin for Transaction construction and signing.
            //Redeem script+TxOut+OutPoint=>ScriptCoin.
            ScriptCoin scriptCoin = received.Outputs.AsCoins().First().ToScriptCoin(redeemScript1);



            //The rest of the code concerning transaction generation and signing is exactly the same as in the previous section about native multi-sig.

            //We will sent the coin to the above nico.

            //Create TransactionBuilder.
            TransactionBuilder builder1 = new TransactionBuilder();
            TransactionBuilder builder1ForA = new TransactionBuilder();
            TransactionBuilder builder1ForB = new TransactionBuilder();

            Transaction unsigned1 =
                builder1
                    .AddCoins(scriptCoin)
                    .Send(nico, Money.Coins(1.0m))
                    .BuildTransaction(sign: false);

            Transaction aliceSigned1 =
                builder1ForA
                    .AddCoins(scriptCoin)
                    .AddKeys(alice)
                    .SignTransaction(unsigned1);

            Transaction bobSigned1 =
                builder1ForB
                    .AddCoins(scriptCoin)
                    .AddKeys(bob)
                    .SignTransaction(unsigned1);



            Transaction fullySigned1 =
                builder
                    .AddCoins(coin)
                    .CombineSignatures(aliceSigned1, bobSigned1);

            Console.WriteLine($"=====fullySigned1=====\n {fullySigned1}");





            // P2SH(P2WPKH)

            Console.WriteLine(key.PubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);

            Console.WriteLine(key.PubKey.ScriptPubKey.WitHash.ScriptPubKey.Hash.ScriptPubKey);

            // Arbitrary

            BitcoinAddress address = BitcoinAddress.Create("1KF8kUVHK42XzgcmJF4Lxz4wcL5WDL97PB");
            var birth = Encoding.UTF8.GetBytes("18/07/1988");
            var birthHash = Hashes.Hash256(birth);
            redeemScript = new Script(
                            "OP_IF "
                                + "OP_HASH256 " + Op.GetPushOp(birthHash.ToBytes()) + " OP_EQUAL " +
                            "OP_ELSE "
                                + address.ScriptPubKey + " " +
                            "OP_ENDIF");

            var tx = new Transaction();
            tx.Outputs.Add(new TxOut(Money.Parse("0.0001"), redeemScript.Hash));
            scriptCoin = tx.Outputs.AsCoins().First().ToScriptCoin(redeemScript);

            //Create spending transaction
            Transaction spending = new Transaction();
            spending.AddInput(new TxIn(new OutPoint(tx, 0)));

            ////Option 1 : Spender knows my birthdate
            Op pushBirthdate = Op.GetPushOp(birth);
            Op selectIf = OpcodeType.OP_1; //go to if
            Op redeemBytes = Op.GetPushOp(redeemScript.ToBytes());
            Script scriptSig = new Script(pushBirthdate, selectIf, redeemBytes);
            spending.Inputs[0].ScriptSig = scriptSig;

            //Verify the script pass
            var result = spending
                            .Inputs
                            .AsIndexedInputs()
                            .First()
                            .VerifyScript(tx.Outputs[0].ScriptPubKey);
            Console.WriteLine(result); // True
                                       ///////////

            ////Option 2 : Spender knows my private key
            BitcoinSecret secret = new BitcoinSecret("...");
            var sig = spending.SignInput(secret, scriptCoin);
            var p2pkhProof = PayToPubkeyHashTemplate
                .Instance
                .GenerateScriptSig(sig, secret.PrivateKey.PubKey);
            selectIf = OpcodeType.OP_0; //go to else
            scriptSig = p2pkhProof + selectIf + redeemBytes;
            spending.Inputs[0].ScriptSig = scriptSig;


            //Verify the script pass
            result = spending
                            .Inputs
                            .AsIndexedInputs()
                            .First()
                            .VerifyScript(tx.Outputs[0].ScriptPubKey);
            Console.WriteLine(result);


            Console.ReadLine();
        }
    }
}
