namespace Docs

import System
import System.IO


parser = XmlParser()

dirName = "../../xml/"

for file in DirectoryInfo(dirName).GetFiles():
	Console.WriteLine("Parsing " + file.FullName)
	parser.Parse(file.FullName)


print "Press any key to continue . . . "
Console.ReadKey(true)
