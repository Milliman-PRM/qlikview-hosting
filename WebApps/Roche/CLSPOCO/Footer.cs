using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLSPOCO
{
    public class Footer
    {
        private List<string> _FooterItems;
        public List<string> FooterItems
        {
            get
            {
                return _FooterItems;
            }

            set
            {
                _FooterItems = value;
            }
        }

        private string _FooterLink;
        public string FooterLink
        {
            get
            {
                return _FooterLink;
            }

            set
            {
                _FooterLink = value;
            }
        }

        private string _FootLinkURI;
        public string FootLinkURI
        {
            get
            {
                return _FootLinkURI;
            }

            set
            {
                _FootLinkURI = value;
            }
        }


        public Footer(bool Test = false )
        {
            _FooterItems = new List<string>();

            if (Test)
            {
                _FooterLink = @"Source: CMS Medicare Clinical Lab Fee Schedule";
                _FootLinkURI = @"https://www.cms.gov/Medicare/Medicare-Fee-for-Service-Payment/ClinicalLabFeeSched/clinlab.html";


                _FooterItems.Add(@"The additional 2% claims payment reduction required under sequestration, effective for dates on service on or after April 1, 2013, remains in effect.");
                _FooterItems.Add(@"Current Procedural Terminology (CPT) copyright 2016 American Medical Association. All rights reserved.  CPT is a registered trademark of the American Medical Association.");
                _FooterItems.Add(@"No fee schedules, basic units, relative values, or related listings are included in CPT. The AMA assumes no liability for the data contained herein.");
                _FooterItems.Add(@"Applicable FARS/DFARS restrictions apply to government use.");
                _FooterItems.Add(@"Subject to special profile and/or panel pricing and payment rules when performed on the same patient on the same date of service. Payment is based on the lower of the billed amount, profile/panel or total of the prices for all covered component tests.");
            }
        }
    }
}
