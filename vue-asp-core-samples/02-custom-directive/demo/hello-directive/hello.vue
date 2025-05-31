<template>
  <div>
    Hello from custom directive!<br />
    <strong>ID:</strong> {{ id }}
  </div>
</template>

<script>
export default {
  name: "hello",
  props: {
    id: { type: String, required: true },
  },
};
</script>
