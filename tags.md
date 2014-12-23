---
layout: page
title: Tags
permalink: /tags/
---

{% for tag in site.tags %} 
  <h2>{{ tag[0] | join: "/" }}</h2>
  <ul>
  	{% for posts in tag %}
      {% for post in posts %}
      {% if post.url %} 
        <li><a href="{{ post.url }}">{{ post.title }}</a></li>
      {% endif %}
      {% endfor %}
    {% endfor %}
  </ul>
{% endfor %}