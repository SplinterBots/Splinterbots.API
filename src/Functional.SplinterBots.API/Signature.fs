namespace Functional.SplinterBots.API

module Signature =

    open System
    open System.Text
    open HiveAPI
    open Cryptography.ECDSA

    let sign username postingKey =
        let ts = 
            let offset = new DateTimeOffset(DateTime.Now)
            offset.ToUnixTimeMilliseconds()
        let hash = $"{username}{ts}" |> Encoding.ASCII.GetBytes |> Sha256Manager.GetHash
        let privatekey = postingKey |> CBase58.DecodePrivateWif
        let rawSignature = Secp256K1Manager.SignCompressedCompact (hash, privatekey)
        let signature = rawSignature |> Hex.ToString
        (ts, signature)
