﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using MiningCore.Configuration;
using MiningCore.Crypto.Hashing.Algorithms;
using MiningCore.Crypto.Hashing.Special;
using Newtonsoft.Json.Linq;

namespace MiningCore.Crypto
{
    public static class HashFactory
    {
        public static IHashAlgorithm GetHash(JObject definition)
        {
            var hash = definition["hash"]?.Value<string>().ToLower();

            if(string.IsNullOrEmpty(hash))
                throw new NotSupportedException("$Invalid or empty hash value {hash}");

            var args = definition["args"]?
                .Select(token => token.Type == JTokenType.Object ? GetHash((JObject)token) : token.Value<object>())
                .ToArray();

            return InstantiateHash(hash, args);
        }

        private static IHashAlgorithm InstantiateHash(string name, object[] args)
        {
            if (name == "digest")
                name = nameof(DigestReverser);

            var hashClass = (typeof(Sha256D).Namespace + "." + name).ToLower();
            var hashType = Type.GetType(hashClass, true, false);

            var container = Program.Container;
            var parameters = args.Select(x =>
            {
                new NamedParameter()
            });

            var result = (IHashAlgorithm) container.Resolve(hashType, parameters);
        }
    }
}
