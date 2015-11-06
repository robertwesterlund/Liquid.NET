﻿using System;
using System.Linq;
using Liquid.NET.Symbols;
using Liquid.NET.Utils;

namespace Liquid.NET.Constants
{
    public class IndexDereferencer
    {
        /// <summary>
        /// Look up the index in the value.  This works for dictionaries, arrays and strings.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="value"></param>
        /// <param name="indexProperty"></param>
        /// <returns></returns>
        public LiquidExpressionResult Lookup(
            ITemplateContext ctx, 
            IExpressionConstant value,
            IExpressionConstant indexProperty)
        {
            var arr = value as ArrayValue;
            if (arr != null)
            {
                return DoLookup(ctx, arr, indexProperty);
            }

            var dict = value as DictionaryValue;
            if (dict != null)
            {
                return DoLookup(ctx, dict, indexProperty);
            }

            var str = value as StringValue;
            if (str != null)
            {
                return DoLookup(ctx, str, indexProperty);
            }

            return LiquidExpressionResult.Error("ERROR : cannot apply an index to a " + value.LiquidTypeName + ".");
        }

        private LiquidExpressionResult DoLookup(ITemplateContext ctx, ArrayValue arrayValue, IExpressionConstant indexProperty)
        {
            bool errorOnEmpty = ctx.Options.ErrorWhenValueMissing && arrayValue.Count == 0;

                            

            String propertyNameString = ValueCaster.RenderAsString(indexProperty);
            int index;
            if (propertyNameString.ToLower().Equals("first"))
            {
                if (errorOnEmpty)
                {
                    return LiquidExpressionResult.Error("cannot dereference empty array");
                }
                index = 0;
            }
            else if (propertyNameString.ToLower().Equals("last"))
            {
                if (errorOnEmpty)
                {
                    return LiquidExpressionResult.Error("cannot dereference empty array");
                }
                index = arrayValue.Count - 1;
            }
            else if (propertyNameString.ToLower().Equals("size"))
            {
                return LiquidExpressionResult.Success(NumericValue.Create(arrayValue.Count));
            }
            else
            {
                var success = Int32.TryParse(propertyNameString, out index);
                //var maybeIndexResult = ValueCaster.Cast<IExpressionConstant, NumericValue>(indexProperty);

                if (!success)
                {
                    if (ctx.Options.ErrorWhenValueMissing)
                    {
                        return LiquidExpressionResult.Error("invalid index: '" + propertyNameString + "'");
                    }
                    else
                    {
                        return LiquidExpressionResult.Success(new None<IExpressionConstant>());// liquid seems to return nothing when non-int index.
                    }
                }

//                if (maybeIndexResult.IsError || !maybeIndexResult.SuccessResult.HasValue)
//                {
//                    return LiquidExpressionResult.Error("invalid array index: " + propertyNameString);
//                }
//                else
//                {
//                    index = maybeIndexResult.SuccessValue<NumericValue>().IntValue;
//                }
            }

            if (arrayValue.Count == 0)
            {
                return errorOnEmpty ? 
                    LiquidExpressionResult.Error("cannot dereference empty array") : 
                    LiquidExpressionResult.Success(new None<IExpressionConstant>());
            }
            var result = arrayValue.ValueAt(index);

            return LiquidExpressionResult.Success(result);
        }

        private LiquidExpressionResult DoLookup(ITemplateContext ctx, DictionaryValue dictionaryValue, IExpressionConstant indexProperty)
        {

            String propertyNameString = ValueCaster.RenderAsString(indexProperty);
            if (propertyNameString.ToLower().Equals("size"))
            {
                return LiquidExpressionResult.Success(NumericValue.Create(dictionaryValue.Keys.Count));
            }

            var valueAt = dictionaryValue.ValueAt(indexProperty.Value.ToString());
            if (valueAt.HasValue)
            {
                return LiquidExpressionResult.Success(valueAt);
            }
            else
            {
                return LiquidExpressionResult.ErrorOrNone(ctx, indexProperty.ToString());

            }
        }

        // TODO: this is inefficient and ugly and duplicates much of ArrayValue
        private LiquidExpressionResult DoLookup(ITemplateContext ctx, StringValue strValue, IExpressionConstant indexProperty)
        {
            var strValues = strValue.StringVal.ToCharArray().Select(ch => new StringValue(ch.ToString()).ToOption()).ToList();
            String propertyNameString = ValueCaster.RenderAsString(indexProperty);
            int index;
            if (propertyNameString.ToLower().Equals("first"))
            {
                index = 0;
            }
            else if (propertyNameString.ToLower().Equals("last"))
            {
                index = strValues.Count - 1;
            }
            else if (propertyNameString.ToLower().Equals("size"))
            {
                return LiquidExpressionResult.Success(NumericValue.Create(strValues.Count));
            }
            else
            {
                var maybeIndexResult = ValueCaster.Cast<IExpressionConstant, NumericValue>(indexProperty);
                if (maybeIndexResult.IsError || !maybeIndexResult.SuccessResult.HasValue)
                {
                    return LiquidExpressionResult.Error("invalid array index: " + propertyNameString);
                }
                else
                {
                    index = maybeIndexResult.SuccessValue<NumericValue>().IntValue;
                }
            }

            if (strValues.Count == 0)
            {
                //return LiquidExpressionResult.Error("Empty string: " + propertyNameString);
                return LiquidExpressionResult.Success(new None<IExpressionConstant>()); // not an error in Ruby liquid.
            }
            return LiquidExpressionResult.Success(ArrayIndexer.ValueAt(strValues, index));

        }

    }
}
