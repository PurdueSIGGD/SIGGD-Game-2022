# SIGGD-Unity-Template
Template for creating new Unity GitHub repositories.
How to setup: (Note that only one person needs to do this setup)
  1) Click the "Use this template" button to make your own duplicate repository on GitHub
  2) Make sure to rename this repository to whatever you want your project to be called
  3) Download and setup the new reposotory to your computer (Using GitHub Desktop is reccomended)
  4) Create a new Unity project in the repository folder
  5) Move the Unity project files so that they are not in a subfolder of the repository folder (the gitignore will not work otherwise)
  6) Commit the changes to the Master branch
  7) The repository should now be ready for use by anyone

How to use once setup:
  1) Make sure other members of the team have the same version of Unity installed as the one the project is
  2) Have them install this repo on their computer (Using GitHub Desktop is reccomended)
  3) In Unity Hub, click "Open" in the "Projects" tab then select the repo folder
  4) This should allow them to have a Unity project based on the repo and allow Git on their own computer to track their changes

General GitHub suggestions:
  1) Have members of your team each create branches for all new changes after this to avoid overwriting anyone's work
  2) Once something on a branch is "done" pull the latest branches from master into your own branch
  3) Check that your branch still works with the newest master changes
  4) Create a pull request from your branch into master
  5) These pull requests will be checked by one of the project leads and merged if able, if not, marge master into your own branch again and fix any conflicts

Additional notes:
  1) The GitIgnore is correct as of 9/15/2022 and should work for the 2022.x.xx versions of Unity. It is unlikely that it will change much, but if files are not being ignored properly, replace the code in the GitIgnore with the code here: https://github.com/github/gitignore/blob/master/Unity.gitignore (Also note that the GitIgnore will only work if the Unity project is at the top level of the repo, rather than in a subfolder).
