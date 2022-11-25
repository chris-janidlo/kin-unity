#[no_mangle]
pub extern "C" fn mul_by_5(i: i32) -> i32 {
    i * 5
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn it_works() {
        let result = mul_by_5(2);
        assert_eq!(result, 10);
    }
}
