using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

namespace ShareAll
{
    class TwitterRegex
    {
        private static readonly string UNICODE_SPACES = "[" + "\\u0009-\\u000d" + "\\u0020" + "\\u0085" + "\\u00a0" + "\\u1680" + "\\u180E" + "\\u2000-\\u200a" + "\\u2028" + "\\u2029" + "\\u202F" + "\\u205F" + "\\u3000" + "]"; // White_Space # Zs       IDEOGRAPHIC SPACE -  White_Space # Zs       MEDIUM MATHEMATICAL SPACE -  White_Space # Zs       NARROW NO-BREAK SPACE -  White_Space # Zp       PARAGRAPH SEPARATOR -  White_Space # Zl       LINE SEPARATOR -  # White_Space # Zs  [11] EN QUAD..HAIR SPACE -  White_Space # Zs       MONGOLIAN VOWEL SEPARATOR -  White_Space # Zs       OGHAM SPACE MARK -  White_Space # Zs       NO-BREAK SPACE -  White_Space # Cc       <control-0085> -  White_Space # Zs       SPACE -   # White_Space # Cc   [5] <control-0009>..<control-000D>

        private static string LATIN_ACCENTS_CHARS = "\\u00c0-\\u00d6\\u00d8-\\u00f6\\u00f8-\\u00ff" + "\\u0100-\\u024f" + "\\u0253\\u0254\\u0256\\u0257\\u0259\\u025b\\u0263\\u0268\\u026f\\u0272\\u0289\\u028b" + "\\u02bb" + "\\u0300-\\u036f" + "\\u1e00-\\u1eff"; // Latin Extended Additional (mostly for Vietnamese) -  Combining diacritics -  Hawaiian -  IPA Extensions -  Latin Extended A and B -  Latin-1
        private static readonly string HASHTAG_ALPHA_CHARS = "a-z" + LATIN_ACCENTS_CHARS + "\\u0400-\\u04ff\\u0500-\\u0527" + "\\u2de0-\\u2dff\\ua640-\\ua69f" + "\\u0591-\\u05bf\\u05c1-\\u05c2\\u05c4-\\u05c5\\u05c7" + "\\u05d0-\\u05ea\\u05f0-\\u05f4" + "\\ufb1d-\\ufb28\\ufb2a-\\ufb36\\ufb38-\\ufb3c\\ufb3e\\ufb40-\\ufb41" + "\\ufb43-\\ufb44\\ufb46-\\ufb4f" + "\\u0610-\\u061a\\u0620-\\u065f\\u066e-\\u06d3\\u06d5-\\u06dc" + "\\u06de-\\u06e8\\u06ea-\\u06ef\\u06fa-\\u06fc\\u06ff" + "\\u0750-\\u077f\\u08a0\\u08a2-\\u08ac\\u08e4-\\u08fe" + "\\ufb50-\\ufbb1\\ufbd3-\\ufd3d\\ufd50-\\ufd8f\\ufd92-\\ufdc7\\ufdf0-\\ufdfb" + "\\ufe70-\\ufe74\\ufe76-\\ufefc" + "\\u200c" + "\\u0e01-\\u0e3a\\u0e40-\\u0e4e" + "\\u1100-\\u11ff\\u3130-\\u3185\\uA960-\\uA97F\\uAC00-\\uD7AF\\uD7B0-\\uD7FF" + "\\p{InHiragana}\\p{InKatakana}" + "\\p{InCJKUnifiedIdeographs}" + "\\u3003\\u3005\\u303b" + "\\uff21-\\uff3a\\uff41-\\uff5a" + "\\uff66-\\uff9f" + "\\uffa1-\\uffdc"; // half width Hangul (Korean) -  half width Katakana -  full width Alphabet -  Kanji/Han iteration marks -  Japanese Kanji / Chinese Han -  Japanese Hiragana and Katakana -  Hangul (Korean) -  Thai -  Zero-Width Non-Joiner -  Pres. Forms B -  Pres. Forms A -  Arabic Supplement and Extended A -  Arabic -  Hebrew Pres. Forms -  Hebrew -  Cyrillic Extended A/B -  Cyrillic
        private static readonly string HASHTAG_ALPHA_NUMERIC_CHARS = "0-9\\uff10-\\uff19_" + HASHTAG_ALPHA_CHARS;
        private static readonly string HASHTAG_ALPHA = "[" + HASHTAG_ALPHA_CHARS + "]";
        private static readonly string HASHTAG_ALPHA_NUMERIC = "[" + HASHTAG_ALPHA_NUMERIC_CHARS + "]";

        /* URL related hash regex collection */
        private const string URL_VALID_PRECEEDING_CHARS = "(?:[^A-Z0-9@＠$#＃\u202A-\u202E]|^)";

        private static readonly string URL_VALID_CHARS = "[\\p{Alnum}" + LATIN_ACCENTS_CHARS + "]";
        private static readonly string URL_VALID_SUBDOMAIN = "(?:(?:" + URL_VALID_CHARS + "[" + URL_VALID_CHARS + "\\-_]*)?" + URL_VALID_CHARS + "\\.)";
        private static readonly string URL_VALID_DOMAIN_NAME = "(?:(?:" + URL_VALID_CHARS + "[" + URL_VALID_CHARS + "\\-]*)?" + URL_VALID_CHARS + "\\.)";
        /* Any non-space, non-punctuation characters. \p{Z} = any kind of whitespace or invisible separator. */
        private const string URL_VALID_UNICODE_CHARS = "[.[^\\p{Punct}\\s\\p{Z}\\p{InGeneralPunctuation}]]";

        private const string URL_VALID_GTLD = "(?:(?:aero|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|xxx)(?=\\P{Alnum}|$))";
        private static readonly string URL_VALID_CCTLD = "(?:(?:ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|" + "bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|" + "er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|" + "hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|" + "lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|" + "nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|" + "sl|sm|sn|so|sr|ss|st|su|sv|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|" + "va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|za|zm|zw)(?=\\P{Alnum}|$))";
        private const string URL_PUNYCODE = "(?:xn--[0-9a-z]+)";

        private static readonly string URL_VALID_DOMAIN = "(?:" + URL_VALID_SUBDOMAIN + "+" + URL_VALID_DOMAIN_NAME + "(?:" + URL_VALID_GTLD + "|" + URL_VALID_CCTLD + "|" + URL_PUNYCODE + ")" + ")" + "|(?:" + URL_VALID_DOMAIN_NAME + "(?:" + URL_VALID_GTLD + "|" + URL_PUNYCODE + ")" + ")" + "|(?:" + "(?<=https?://)" + "(?:" + "(?:" + URL_VALID_DOMAIN_NAME + URL_VALID_CCTLD + ")" + "|(?:" + URL_VALID_UNICODE_CHARS + "+\\." + "(?:" + URL_VALID_GTLD + "|" + URL_VALID_CCTLD + ")" + ")" + ")" + ")" + "|(?:" + URL_VALID_DOMAIN_NAME + URL_VALID_CCTLD + "(?=/)" + ")"; // e.g. t.co/ -  domain + ccTLD + '/' -  protocol + unicode domain + TLD -  protocol + domain + ccTLD -  e.g. twitter.com -  domain + gTLD -  e.g. www.twitter.com, foo.co.jp, bar.co.uk -  subdomains + domain + TLD

        private const string URL_VALID_PORT_NUMBER = "[0-9]++";

        private static readonly string URL_VALID_GENERAL_PATH_CHARS = "[a-z0-9!\\*';:=\\+,.\\$/%#\\[\\]\\-_~\\|&" + LATIN_ACCENTS_CHARS + "]";
        /// <summary>
        /// Allow URL paths to contain balanced parens
        ///  1. Used in Wikipedia URLs like /Primer_(film)
        ///  2. Used in IIS sessions like /S(dfd346)/
        /// 
        /// </summary>
        private static readonly string URL_BALANCED_PARENS = "\\(" + URL_VALID_GENERAL_PATH_CHARS + "+\\)";
        /// <summary>
        /// Valid end-of-path chracters (so /foo. does not gobble the period).
        ///   2. Allow =&# for empty URL parameters and other URL-join artifacts
        /// 
        /// </summary>
        private static readonly string URL_VALID_PATH_ENDING_CHARS = "[a-z0-9=_#/\\-\\+" + LATIN_ACCENTS_CHARS + "]|(?:" + URL_BALANCED_PARENS + ")";

        private static readonly string URL_VALID_PATH = "(?:" + "(?:" + URL_VALID_GENERAL_PATH_CHARS + "*" + "(?:" + URL_BALANCED_PARENS + URL_VALID_GENERAL_PATH_CHARS + "*)*" + URL_VALID_PATH_ENDING_CHARS + ")|(?:@" + URL_VALID_GENERAL_PATH_CHARS + "+/)" + ")";

        private const string URL_VALID_URL_QUERY_CHARS = "[a-z0-9!?\\*'\\(\\);:&=\\+\\$/%#\\[\\]\\-_\\.,~\\|]";
        private const string URL_VALID_URL_QUERY_ENDING_CHARS = "[a-z0-9_&=#/]";
        private static readonly string VALID_URL_PATTERN_STRING = "(" + "(" + URL_VALID_PRECEEDING_CHARS + ")" + "(" + "(https?://)?" + "(" + URL_VALID_DOMAIN + ")" + "(?::(" + URL_VALID_PORT_NUMBER + "))?" + "(/" + URL_VALID_PATH + "*+" + ")?" + "(\\?" + URL_VALID_URL_QUERY_CHARS + "*" + URL_VALID_URL_QUERY_ENDING_CHARS + ")?" + ")" + ")"; //  $8 Query String -   $7 URL Path and anchor -   $6 Port number (optional) -   $5 Domain(s) -   $4 Protocol (optional) -   $3 URL -   $2 Preceeding chracter -   $1 total match

        private static string AT_SIGNS_CHARS = "@\uFF20";

        private const string DOLLAR_SIGN_CHAR = "\\$";
        private const string CASHTAG = "[a-z]{1,6}(?:[._][a-z]{1,2})?";

        /* Begin public constants */
        public static Regex VALID_HASHTAG = new Regex("(^|[^&" + HASHTAG_ALPHA_NUMERIC_CHARS + "])(#|\uFF03)(" + HASHTAG_ALPHA_NUMERIC + "*" + HASHTAG_ALPHA + HASHTAG_ALPHA_NUMERIC + "*)", RegexOptions.IgnoreCase);
        public const int VALID_HASHTAG_GROUP_BEFORE = 1;
        public const int VALID_HASHTAG_GROUP_HASH = 2;
        public const int VALID_HASHTAG_GROUP_TAG = 3;
        public static Regex INVALID_HASHTAG_MATCH_END = new Regex("^(?:[#＃]|://)");

        public static Regex AT_SIGNS = new Regex("[" + AT_SIGNS_CHARS + "]");
        public static Regex VALID_MENTION_OR_LIST = new Regex("([^a-z0-9_!#$%&*" + AT_SIGNS_CHARS + "]|^|RT:?)(" + AT_SIGNS + "+)([a-z0-9_]{1,20})(/[a-z][a-z0-9_\\-]{0,24})?", RegexOptions.IgnoreCase);
        public const int VALID_MENTION_OR_LIST_GROUP_BEFORE = 1;
        public const int VALID_MENTION_OR_LIST_GROUP_AT = 2;
        public const int VALID_MENTION_OR_LIST_GROUP_USERNAME = 3;
        public const int VALID_MENTION_OR_LIST_GROUP_LIST = 4;

        public static Regex VALID_REPLY = new Regex("^(?:" + UNICODE_SPACES + ")*" + AT_SIGNS + "([a-z0-9_]{1,20})", RegexOptions.IgnoreCase);
        public const int VALID_REPLY_GROUP_USERNAME = 1;

        public static Regex INVALID_MENTION_MATCH_END = new Regex("^(?:[" + AT_SIGNS_CHARS + LATIN_ACCENTS_CHARS + "]|://)");

        public static Regex VALID_URL = new Regex(VALID_URL_PATTERN_STRING, RegexOptions.IgnoreCase);
        public const int VALID_URL_GROUP_ALL = 1;
        public const int VALID_URL_GROUP_BEFORE = 2;
        public const int VALID_URL_GROUP_URL = 3;
        public const int VALID_URL_GROUP_PROTOCOL = 4;
        public const int VALID_URL_GROUP_DOMAIN = 5;
        public const int VALID_URL_GROUP_PORT = 6;
        public const int VALID_URL_GROUP_PATH = 7;
        public const int VALID_URL_GROUP_QUERY_STRING = 8;

        public static Regex VALID_TCO_URL = new Regex("^https?:\\/\\/t\\.co\\/[a-z0-9]+", RegexOptions.IgnoreCase);
        public static Regex INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN = new Regex("[-_./]$");

        public static Regex VALID_CASHTAG = new Regex("(^|" + UNICODE_SPACES + ")(" + DOLLAR_SIGN_CHAR + ")(" + CASHTAG + ")" + "(?=$|\\s|\\p{Punct})", RegexOptions.IgnoreCase);
        public const int VALID_CASHTAG_GROUP_BEFORE = 1;
        public const int VALID_CASHTAG_GROUP_DOLLAR = 2;
        public const int VALID_CASHTAG_GROUP_CASHTAG = 3;


        public IList<string> extractURLsWithIndices(string text)
        {
            bool extractURLWithoutProtocol = true;
            if (text == null || text.Length == 0 || (extractURLWithoutProtocol ? text.IndexOf('.') : text.IndexOf(':')) == -1)
            {
                // Performance optimization.
                // If text doesn't contain '.' or ':' at all, text doesn't contain URL,
                // so we can simply return an empty list.
                return Collections.emptyList();
            }

            IList<string> urls = new List<string>();

            MatchCollection matcher = TwitterRegex.VALID_URL.Matches(text);
            //Matcher matcher = TwitterRegex.VALID_URL.matcher(text);

            foreach (Match match in matcher)

            while (matcher.find())
            {
                if (matcher.group(TwitterRegex.VALID_URL_GROUP_PROTOCOL) == null)
                {
                    // skip if protocol is not present and 'extractURLWithoutProtocol' is false
                    // or URL is preceded by invalid character.
                    if (!extractURLWithoutProtocol || TwitterRegex.INVALID_URL_WITHOUT_PROTOCOL_MATCH_BEGIN.matcher(matcher.group(TwitterRegex.VALID_URL_GROUP_BEFORE)).matches())
                    {
                        continue;
                    }
                }
                string url = matcher.group(TwitterRegex.VALID_URL_GROUP_URL);
                int start = matcher.start(TwitterRegex.VALID_URL_GROUP_URL);
                int end = matcher.end(TwitterRegex.VALID_URL_GROUP_URL);
                Matcher tco_matcher = TwitterRegex.VALID_TCO_URL.matcher(url);
                if (tco_matcher.find())
                {
                    // In the case of t.co URLs, don't allow additional path characters.
                    url = tco_matcher.group();
                    end = start + url.Length;
                }

                urls.Add(new string(start, end, url, string.Type.URL));
            }

            return urls;
        }
    }

    

}
