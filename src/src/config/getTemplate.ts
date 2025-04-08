import { useEffect, useState } from "react";

export function getTemplate(id: any) {
  if (!id) return null;
  const [data, setData] = useState({});
  useEffect(() => {
    import(`../templates/${id}.json`)
      .then((res) => setData(res.default))
      .catch(_ => null);
  }, []);
  return data;
}
