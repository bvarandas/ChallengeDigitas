using FluentResults;
using System.Reflection.Metadata.Ecma335;

namespace OrderBook.Core.ValuesObject;
public record Ticker(Crypto crypto, Currency currency);
