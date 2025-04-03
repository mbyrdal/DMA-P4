<template>
    <div>
      <h1>Item List</h1>
      <ul>
        <li v-for="item in items" :key="item.ID">
          {{ item.Navn }} - {{ item.Antal }} in {{ item.Lokation }}
        </li>
      </ul>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  
  export default {
    data() {
      return {
        items: [],
      };
    },
    async created() {
      try {
        const response = await axios.get("http://localhost:7092/api/item");
        this.items = response.data;
      } catch (error) {
        console.error("Error fetching items:", error);
      }
    },
  };
  </script>
  