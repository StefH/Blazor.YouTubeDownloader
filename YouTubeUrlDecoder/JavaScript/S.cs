namespace YouTubeUrlDecoder.JavaScript
{
    internal static class S
    {
        public static string Code => @"
        var fB = {
            RP: function (a, b) {
                a.splice(0, b)
            },
            Td: function (a) {
                a.reverse()
            },
            kq: function (a, b) {
                var c = a[0];
                a[0] = a[b % a.length];
                a[b % a.length] = c
            }
        };
        var bla = function (a) {
            a = a.split('');
            fB.kq(a, 35);
            fB.RP(a, 2);
            fB.kq(a, 46);
            fB.Td(a, 6);
            return a.join('')
        };;
        bla(s__);";
    }
}
