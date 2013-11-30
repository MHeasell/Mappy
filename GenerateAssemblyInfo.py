from subprocess import check_output
import re
import string
import sys
import os.path

projectdir = sys.argv[1]
inpath = os.path.join(projectdir, "Properties/AssemblyInfo.cs.tmpl")
outpath = os.path.join(projectdir, "Properties/AssemblyInfo.cs")

# get the tag
tag = check_output(["git", "describe", "--dirty=-d"])

# grab main version blob (x.y.z) and git extras (w-<hash>)
main_match = re.match("v([^\-]+)(?:-([0-9a-z\-]+))?", tag)
main_version_text = main_match.group(1)
extra_text = main_match.group(2)

# parse version blob
version_match = re.match("([0-9]+)\.([0-9]+)(?:\.([0-9]+))?", main_version_text)
major = version_match.group(1)
minor = version_match.group(2)
hotfix = version_match.group(3) or "0"

# optionally parse git extras
revision = "0"
hashref = None
if (extra_text != None):
    parts = extra_text.split("-", 1)
    if len(parts) > 1:
        revision = parts[0]
        hashref = parts[1]
    else:
        hashref = parts[0]

# build the output version string
version = string.join([major, minor, hotfix, revision], ".")

# append hashref to infoversion if we have one
infoversion = version
if (hashref != None):
    infoversion += "-" + hashref

# process the template file
f = open(inpath, "r")
out = open(outpath, "w")
for line in f:
    line = line.replace("{VERSION}", version)
    line = line.replace("{INFOVERSION}", infoversion)
    out.write(line)
out.close()
f.close()
