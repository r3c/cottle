# -*- coding: utf-8 -*-

"""
Resources:

https://pythonhosted.org/an_example_pypi_project/sphinx.html
https://github.com/djungelorm/sphinx-csharp
https://sublime-and-sphinx-guide.readthedocs.io/en/latest/code_blocks.html
https://docutils.sourceforge.net/docs/user/rst/quickref.html
"""

extensions = [
    'sphinx_csharp.csharp',
    'sphinx_rtd_theme'
]

# Add any paths that contain templates here, relative to this directory.
templates_path = ['_templates']

# The suffix of source filenames.
source_suffix = '.rst'

# The master toctree document.
master_doc = 'index'

# General information about the project.
project = u'Cottle Documentation'
copyright = u'2019, Rémi Caput'

# The short X.Y version.
version = '2.0'
# The full version, including alpha/beta/rc tags.
release = '2.0.0'

# List of patterns, relative to source directory, that match files and
# directories to ignore when looking for source files.
exclude_patterns = ['_build']

# The name of the Pygments (syntax highlighting) style to use.
pygments_style = 'default'

# See: https://sphinx-rtd-theme.readthedocs.io/en/stable/configuring.html
html_theme = 'sphinx_rtd_theme'
html_theme_path = ['_themes']
html_theme_options = {
    'style_external_links': True
}

html_logo = '../res/icon.png'

# Add any paths that contain custom static files (such as style sheets) here,
# relative to this directory. They are copied after the builtin static files,
# so a file named "default.css" will overwrite the builtin "default.css".
html_static_path = ['_static']

# Output file base name for HTML help builder.
htmlhelp_basename = 'CottleDocumentation'

# Grouping the document tree into LaTeX files. List of tuples
# (source start file, target name, title,
#  author, documentclass [howto, manual, or own class]).
latex_documents = [
    ('index', 'Cottle.tex', u'Cottle Documentation',
     u'Cottle', 'manual'),
]

# One entry per manual page. List of tuples
# (source start file, name, description, authors, manual section).
man_pages = [
    ('index', 'cottle', u'Cottle Documentation',
     [u'Cottle'], 1)
]

# Grouping the document tree into Texinfo files. List of tuples
# (source start file, target name, title, author,
#  dir menu entry, description, category)
texinfo_documents = [
    ('index', 'Cottle', u'Cottle Documentation',
     u'Cottle', 'Cottle', 'Cottle Documentation.',
     'Miscellaneous'),
]
