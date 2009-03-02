namespace Docs

import System
import System.Xml


// parse the docs XML file
doc = XmlDocument()
doc.Load('Docs.xml')

print "Press any key to continue . . . "
Console.ReadKey(true)
