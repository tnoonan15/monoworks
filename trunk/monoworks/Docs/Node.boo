namespace Docs

import System
import System.Collections.Generic

class Node:
"""Base node for the internal documentation tree."""
	def constructor():
		pass
	
	// The node name.
	public Name as string
	
	// The node summary.
	public Summary as string
		
	// The child nodes.
	public Children = List[of Node]()