<#
TODO
there should really be an "uber" script which ties together all of the scripts, and waits for user input when necessary, and picks up where they left off if possible
you don't have to fully automate it right now, you *can* for example just present each commit and ask the user to tell you which category it is in, and then automate the rest in the future
automation that is still missing:
1. the release PR is not actually created, just a branch created and push and a note of the URL to use to manually create the PR
2. release note generation is not fully automated; categories could be discovered by looking at work items linked to the PR
3. anything after release note generation hasn't been automated yet
#>


### https://stackoverflow.com/questions/54391120/how-to-download-the-latest-build-artifacts-from-azure-devops-programmatically