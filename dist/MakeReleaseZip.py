from subprocess import check_output
import zipfile

import os
import os.path
from os.path import join
import sys
import shutil

release_mode = False

slndir = ".."
projdir = join(slndir, "Mappy")
if (release_mode):
    bindir = join(projdir, "bin/Release")
else:
    bindir = join(projdir, "bin/Debug")

dist_files = [
        join(bindir, "Geometry.dll"),
        join(bindir, "Geometry.pdb"),
        join(bindir, "HPIUtil.dll"),
        join(slndir, "LICENSE"),
        join(bindir, "Mappy.exe"),
        join(bindir, "Mappy.exe.config"),
        join(bindir, "Mappy.pdb"),
        join(slndir, "README.md"),
        join(bindir, "TAUtil.dll"),
        join(bindir, "TAUtil.pdb"),
    ]

project_name = "mappy"

tag = check_output(["git", "describe", "--dirty=-d"]).strip()

dist_name = project_name + "-" + tag

zip_name = dist_name + ".zip"

dist_dir = dist_name

# build dist dir
if (os.path.exists(dist_dir)):
    shutil.rmtree(dist_dir)
os.mkdir(dist_dir)

for path in dist_files:
    shutil.copy(path, dist_dir)

# zip it up
zip_file = zipfile.ZipFile(zip_name, "w")
for (dirpath, dirnames, filenames) in os.walk(dist_dir):
    for f in filenames:
        zip_file.write(join(dirpath, f))
zip_file.close()
