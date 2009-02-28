namespace Docs

import System
import System.Xml

class XmlParser:
"""The XmlParser parses the XML doc files into an internal tree."""
	def constructor():
		pass
	
	def Parse(fileName as string) as AssemblyNode:
	""" Parse an XML file containing automatically generated documentation."""
		reader = XmlTextReader(fileName)
		
		asm = AssemblyNode()		
		
		while not reader.EOF:
			reader.Read()
			if reader.NodeType == XmlNodeType.Element:
				if reader.Name == "assembly":
					reader.Read()
					asm.Name = reader.Value
				//elif reader.Name == "member":
					//asm.Children.Add(ParseMember(reader))
		
		
		return asm
		
		
	//protected def ParseMember(reader as XmlReader) as Node:
	//""" Parses a member into a node depending on what type of member it is."""
		//typeString = reader.GetAttribute("name")[0]
		//if typeString == "T"
			//return ClassNode(reader)
		//elif
			//return Node()
