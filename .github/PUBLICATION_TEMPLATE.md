## Publication Summary

**Note the server to be used for reduction:** 
 - [ ] (e.g. `indy-ss01` or `PRM2`)

| Client Info | Value |
| :---------- | :---- |
| Contact Name/Email | John Doe; john.doe@acme.com |

## Task Lists

#### Tasks to be completed immediately when opening this issue
- [ ] Issue title is "Internal Client - Project Code"  
      e.g. `New York` - `LIVE BPCI - USPI`
- [ ] Filesystem location on the server where the new report is located:**  
      e.g. `D:\0000EXT01_NetworkShare\...`
- [ ] Folder as seen on PMC where the project can be found:**  
      e.g. `0000EXT01` | `NewYork` | `LIVE BPCI - CJR`

#### Tasks to be completed during reduction
- [ ] Make a backup copy of the report provided by the client
- [ ] Use the signature tool to verify that the Report (QVW) has not been signed
- [ ] Inspect the `sign.bat` file and ensure that the qvw file name in the script matches the actual file name
- [ ] Sign the QVW by running the sign.bat script file
- [ ] Use the signature tool to verify the presence of the new signature on the QVW
- [ ] If planning to perform reduction on a server other than where it is, copy the report to the same named folder on the reduction server (e.g. PRM2).  
- [ ] Re-populate the Report (QVW) using the PMC
- [ ] Update Project using the PMC
- [ ] If this is a demo report, skip ahead to procedure for pushing to production

#### Tasks to be completed during validation 
*Not necessary to complete the following tasks if this is a Demo*
- [ ] Validate the Report using the `MillimanQVSelectionValidation` tool
- [ ] Check the validation messages.  
  - Any items noted as 'missing' are generally acceptable.  
  - Any items noted as 'PHI Breach' or 'Previously Not Authorized' should be scrutinized.  

| Message | To Do |
| :------ | :---- |
| **---** | Nothing - this is an OK to push |
| **[MISSING]** for all selections of a user | Leave a comment listing the users with this message below |
| **[MISSING]** for some, but not all selections of a user | Nothing - valid state |
| **PHI Breach** | Leave a comment listing the users with this message below |

#### Tasks to be completed for pushing to production
- [ ] Push the report (QVW) to production
- [ ] Confirm no errors occurred during the push to production 

#### Tasks to be completed when closing this issue
- [ ] Delete the backup copy of the original report provided by the client
- [ ] All checks have been performed
- [ ] Close this issue


#### All additional information should be in separate comments below.  Nothing should be written below this line.
*For instructions on publishing a report (QVW), visit:* https://indy-github.milliman.com/PRM/qlikview-hosting/wiki/Publishing-Updates-to-Existing-Reports-For-Internal-Milliman-Hosting-Clients
