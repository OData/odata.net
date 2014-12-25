---
layout: page
title: Samples
permalink: /samples/
---

{% for category in site.categories %} 
  <h2>{{ category[0] | join: "/" }}</h2>
  <ul>
  	{% for posts in category %}
      {% for post in posts reversed %}
      {% if post.url %} 
        <li><a href="{{site.baseurl}}{{ post.url }}">{{ post.title }}</a></li>
      {% endif %}
      {% endfor %}
    {% endfor %}
  </ul>
{% endfor %}