using System.Linq;
using System.Text.RegularExpressions;

namespace EverybodyEdits.Lobby
{
    internal class BadWords
    {
        private readonly string[] badWordArray =
            "ahole*,anal,anus*,arse*,arss*,ashole*,asshole,asswipe,ballsdeep,biatch*,*bitch*,blowjob,*bollock*,boner,*boob*,breast*,bukkak*,butthole*,buttwipe*,carpetmunch*,chink*,chode*,choad*,clit*,*cnts*,cock*,cok,cokbite*,cokhead*,cokjock*,cokmunch*,coks,coksuck*,coksmoke*,cum*,cunnie*,cunnilingus*,cunny*,*cunt*,*dick*,dik,dike*,diks,dildo*,douche*,doosh*,dooch*,*dumbars*,*dumars*,*dumass*,edjaculat*,ejacculat*,ejackulat*,ejaculat*,ejaculit*,ejakulat*,enema*,faeces*,fag*,*fatars*,*fatass*,*fcuk*,feces*,felatio*feltch*,flikker*,*foreskin*,*forskin*,*fvck*,*fuck*,*fudgepack*,*fuk*,*handjob*,hardon*,*hitler*,hoar,honkey*,*jackars*,*jackass*,*jackoff*,*jerkoff*,jiss*,*jizz*,kike*,knobjock*,knobrid*,knobsuck*,kooch*,kootch*,kraut*,kunt*,kyke*,*lardars*,*lardass*,lesbo*,lessie*,lezzie*,lezzy*,*masturbat*,minge*,minging*,mofo,muffdive*,munge*,munging*,*nazi*,*negro*,niga,nigg*,niglet*,nutsack*,panooch,pecker,peanis,peeenis,peeenus,peeenusss,peenis,peenus,peinus,penas,*penis*,penus,phuc,phuck*,phuk*,pissflap,poon,poonani,poonanni,poonanny,poonany,poontang,porn*,pula,pule,punani,punanni,punanny,punany,pusse,pussee,pussie,pussies,pussy,pussying,pussylick,pussysuck,poossy*,poossie*,puuke,puuker,queef*,queer*,qweer*,qweir*,recktum*,rectum*,renob,retard*,rimming*,rimjob*,ruski*,sadist*,scank*,schlong*,schmuck*,scrote*,scrotum*,semen*,sex*,shemale*,shat,*shit*,skank*,*slut*,spic,spick,spik,tard,teets,testic*,tit,tits,titt,titty*,tittie*,twat*,*vagina*,*vaginna*,*vaggina*,*vaagina*,*vagiina*,vag,vags,vaj,vajs,*vajina*,*vulva*,wank*,*whoar*,whoe,*whor*,*B=D*,*B==D*,*B===*,*===D,8===*"
                .ToLower().Split(',');

        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
                Replace("\\*", ".*").
                Replace("\\?", ".") + "$";
        }

        public static bool ContainsBadWord(string input)
        {
            return !string.IsNullOrEmpty(input)
                   && new BadWords().badWordArray.Any(word =>
                       Regex.IsMatch(input, WildcardToRegex(word),
                           RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
        }
    }
}