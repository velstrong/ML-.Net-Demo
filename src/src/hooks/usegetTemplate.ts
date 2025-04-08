import axios from "axios";
export const fetchTemplate = async (id:string) => {
  try {
    const response = await axios.get(`../../src/templates/${id}.mjml`).then((res) => {
      setTemplate(MjmlToJson(res.data as any));
    });
  } catch (error) {
    console.error(error);
  }
};