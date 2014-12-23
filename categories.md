---
layout: page
title: Categories
permalink: /categories/
---

{% for category in site.categories %} 
  <h2>{{ category[0] | join: "/" }}</h2>
  <ul>
  	{% for posts in category %}
      {% for post in posts %}
      {% if post.url %} 
        <li><a href="{{ post.url }}">{{ post.title }}</a></li>
      {% endif %}
      {% endfor %}
    {% endfor %}
  </ul>
{% endfor %}