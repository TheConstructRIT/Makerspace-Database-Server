/*
 * Zachary Cook
 *
 * Helper functions for security.
 * This mainly handles potential XSS from database entries
 * since they were never sanitized for this.
 */



 /*
  * Removes tags from strings.
  */
function cleanString(uncleanedString) {
    return uncleanedString.replaceAll("&","&amp;").replaceAll("<","&lt;").replaceAll(">","&gt;");
}