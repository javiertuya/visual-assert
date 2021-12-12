package giis.portable;

import java.util.UUID;

/**
 * Funciones para portabilidad entre Java y Csharp. 
 * Deben ser utilizadas en las nativas de Java para los componentes que vayan a ser traducidos a csharp 
 * y usados de forma independiente.
 */
public class JavaCs {
	private JavaCs() {
	    throw new IllegalAccessError("Utility class");
	}
	public static boolean isEmpty(String str) {
		return str==null || "".equals(str.trim());
	}
	public static String getUniqueId() {
		return UUID.randomUUID().toString();
	}
}
