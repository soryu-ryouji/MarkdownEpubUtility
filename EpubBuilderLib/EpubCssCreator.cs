namespace EpubBuilder;

public class CssCreator
{
    public static string GenerateStyleSheet()
    {
        var cssStyles = """
            p {
                font-family: Source Han Sans SC;
                font-size: 1em;
                text-align: justify;
                text-indent: 2em;
                font-weight: normal;
                line-height: 1.84;
                color: #000000
            }

            h1 {
                font-family: Source Han Sans SC;
                font-size: 1.4em;
                text-align: center;
                font-weight: normal;
                line-height: 1.4;
                color: #000000;
            }

            h2 {
                font-family: Source Han Sans SC;
                font-size: 1.28em;
                text-align: center;
                font-weight: normal;
                line-height: 1.4;
                color: #000000;
            }

            h3 {
                font-family: Source Han Sans SC;
                font-size: 1.1em;
                text-align: left;
                font-weight: normal;
                line-height: 1.2;
                color: #000000;
            }

            h4 {
                font-family: Source Han Sans SC;
                font-size: 1em;
                text-align: left;
                font-weight: normal;
                line-height: 1.2;
                color: #000000;
            }

            h5 {
                font-family: Source Han Sans SC;
                font-size: 1em;
                text-align: left;
                font-weight: normal;
                line-height: 1.2;
                color: #000000;
            }

            h6 {
                font-family: Source Han Sans SC;
                font-size: 1em;
                text-align: left;
                font-weight: normal;
                line-height: 1.2;
                color: #000000;
            }
            """;
        return cssStyles;
    }
}