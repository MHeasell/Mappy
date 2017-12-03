from subprocess import check_output
import zipfile

import os
import os.path
from os.path import join
import sys
import shutil
import argparse

parser = argparse.ArgumentParser(description="Release packaging script")
parser.add_argument("--release", dest="release", action="store_const",
        const=True, default=False,
        help="package from release directory")
args = parser.parse_args()

release_mode = args.release

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
        join(bindir, "ICSharpCode.SharpZipLib.dll"),
        join(bindir, "Pngcs.dll"),
        join(slndir, "Pngcs/LICENSE.Pngcs.txt"),
        join(bindir, "Ookii.Dialogs.dll"),
        join(slndir, "Ookii.Dialogs/LICENSE.Ookii.Dialogs.txt"),
        join(slndir, "LICENSE.txt"),
        join(bindir, "Mappy.exe"),
        join(bindir, "Mappy.exe.config"),
        join(bindir, "Mappy.pdb"),
        join(slndir, "README.md"),
        join(slndir, "license.rx.txt"),
        join(bindir, "System.Reactive.Core.dll"),
        join(bindir, "System.Reactive.Interfaces.dll"),
        join(bindir, "System.Reactive.Linq.dll"),
        join(bindir, "System.Reactive.PlatformServices.dll"),
        join(bindir, "System.Reactive.Windows.Forms.dll"),
        join(bindir, "TAUtil.dll"),
        join(bindir, "TAUtil.pdb"),
        join(bindir, "TAUtil.Hpi.dll"),
        join(bindir, "TAUtil.Hpi.pdb"),
        join(bindir, "TAUtil.Gdi.dll"),
        join(bindir, "TAUtil.Gdi.pdb"),
    ]

project_name = "mappy"

tag = check_output(["git", "describe", "--dirty=-d"], universal_newlines=True).strip()

dist_name = project_name + "-" + tag
if not release_mode:
    dist_name += "-DEBUG"

zip_name = dist_name + ".zip"

dist_dir = dist_name

# build dist dir
if (os.path.exists(dist_dir)):
    shutil.rmtree(dist_dir)
os.mkdir(dist_dir)

for path in dist_files:
    shutil.copy(path, dist_dir)

# rename the readme
os.rename(join(dist_dir, "README.md"), join(dist_dir, "README.txt"))

# zip it up
zip_file = zipfile.ZipFile(zip_name, "w", zipfile.ZIP_DEFLATED)
for (dirpath, dirnames, filenames) in os.walk(dist_dir):
    for f in filenames:
        zip_file.write(join(dirpath, f))
zip_file.close()
