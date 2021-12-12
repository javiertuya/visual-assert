package giis.portable;

import java.io.File;
import java.io.FileFilter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.commons.io.FilenameUtils; //no usa java.nio.file.Paths por compatibilidad con java 1.6
import org.apache.commons.io.FileUtils;
import org.apache.commons.io.filefilter.WildcardFileFilter;

/**
 * Metodos de manejo de ficheros para compatibilidad Java/C#
 */
public class FileUtil {
	private static final String UTF_8 = "UTF-8";
	private FileUtil() {
	    throw new IllegalAccessError("Utility class");
	  }
	
	public static String fileRead(String fileName, boolean throwIfNotExists) {
		try {
			File f=new File(fileName);
			if (f.exists())
				return FileUtils.readFileToString(f, UTF_8);
			if (throwIfNotExists)
				throw new RuntimeException("File does not exist "+fileName);
			else
				return null;
		} catch (IOException e) {
			throw new RuntimeException(e);
		}
	}
	public static String fileRead(String fileName) {
		return fileRead(fileName, true);
	}

	public static void fileWrite(String fileName, String contents) {
		try {
			FileUtils.writeStringToFile(new File(fileName), contents, UTF_8);
		} catch (IOException e) {
			throw new RuntimeException("Error writing file "+fileName, e);
		}
	}
	public static List<String> getFileListInDirectory(String path) {
		List<String> lst=new ArrayList<String>();
		try {
			File dir=new File(path);
			for(File file: dir.listFiles()) {
				lst.add(file.getName());
			}
		} catch (RuntimeException e) {
			throw new RuntimeException("Can't browse directory at path " + path);
		}
		return lst;
	}

	public static String getPath(String first, String... more) {
		String result=first;
		//El primer componente no puede empezar de forma relativa como .. (concat devuelve null), por lo que busca el full path primero
		if (result.startsWith("."))
			result=getFullPath(result);
		for (int i=0; i<more.length; i++)
			result=FilenameUtils.concat(result, more[i]);
        return result;
    }
	//Variantes sin array dinamico para compatibilidad con downgrade a jdk4 realizado por retrotranslator
	public static String getPath(String first, String more1) {
		return getPath(first, new String[] {more1}); //NOSONAR necesario por compatibilidad
    }

	public static String getFullPath(String path)
    {
        try {
			return new File(path).getCanonicalPath();
		} catch (IOException e) {
			throw new RuntimeException("Error getting full path of "+path, e);
		}
    }

	/** 
	 * Crea la carpeta indicada como parametro
	 */
	public static void createDirectory(String path) {
		try {
			FileUtils.forceMkdir(new File(path));
		} catch (IOException e) {
			throw new RuntimeException(e);
		}
	}

}
