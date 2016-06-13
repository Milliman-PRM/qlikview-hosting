using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class LocalitySearch
    {
        private List<string> _Localities;

        public List<string> Localities
        {
            get
            {
                return _Localities;
            }

            set
            {
                _Localities = value;
            }
        }

        public LocalitySearch( bool Test = false )
        {
            _Localities = new List<string>();
            if ( Test )
            {
                List<string> All = new List<string>() { "AK - Alaska","AL - Alabama","AR - Arkansas","AZ - Arizona","CA1 - Northern California","CA2 - Southern Cal Occidental","CO - Colorado","CT - Connecticut","DC - District of Columbia","DE - Delaware","FL - Florida","GA - Georgia","HI - Hawaii","IA - Iowa","ID - Idaho","IL - Illinois","IN - Indiana","KS1 - Kansas","KS2 - Kansas","KY - Kentucky","LA - Louisiana","MA - Massachusetts","MD - Maryland","ME - Maine","MI - Michigan","MN - Minnesota","MO1 - Missouri General Amer","MO2 - Missouri","MS - Mississippi","MT - Montana","National Limit","NC - North Carolina","ND - North Dakota","NE - Nebraska","NH - New Hampshire","NJ - New Jersey","NM - New Mexico","NV - Nevada","NY1 - Western New York","NY2 - Empire New York","NY3 - New York GHI","OH - Ohio","OK - Oklahoma","OR - Oregon","PA - Pennsylvania","PR - Puerto Rico","RI - Rhode Island","SC - South Carolina","SD - South Dakota","TN - Tennessee","TX - Texas","UT - Utah","VA - Virginia","VT - Vermont","WA - Washington State","WI - Wisconsin","WV - West Virginia","WY - Wyoming"};
                foreach( string Local in All )
                    _Localities.Add(Local);
               

            }
        }
    }
}
